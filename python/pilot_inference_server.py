import argparse
import base64
import io
import os
from typing import Any

import numpy as np
from flask import Flask, jsonify, request
from PIL import Image

try:
    import tensorflow as tf
except Exception as exc:  # pragma: no cover - runtime environment guard
    tf = None
    TF_IMPORT_ERROR = exc
else:
    TF_IMPORT_ERROR = None


app = Flask(__name__)

MODEL_PATH = ""
KERAS_MODEL = None
TFLITE_INTERPRETER = None
TFLITE_INPUT_INDEX = None
TFLITE_OUTPUT_DETAILS = None
INPUT_HEIGHT = 120
INPUT_WIDTH = 160


def _load_model(model_path: str) -> None:
    global MODEL_PATH
    global KERAS_MODEL
    global TFLITE_INTERPRETER
    global TFLITE_INPUT_INDEX
    global TFLITE_OUTPUT_DETAILS
    global INPUT_HEIGHT
    global INPUT_WIDTH

    if tf is None:
        raise RuntimeError(f"TensorFlow import failed: {TF_IMPORT_ERROR}")
    if not os.path.exists(model_path):
        raise FileNotFoundError(model_path)

    MODEL_PATH = model_path
    lower_path = model_path.lower()

    if lower_path.endswith(".tflite"):
        TFLITE_INTERPRETER = tf.lite.Interpreter(model_path=model_path)
        TFLITE_INTERPRETER.allocate_tensors()
        input_details = TFLITE_INTERPRETER.get_input_details()
        output_details = TFLITE_INTERPRETER.get_output_details()
        shape = input_details[0]["shape"]
        INPUT_HEIGHT = int(shape[1])
        INPUT_WIDTH = int(shape[2])
        TFLITE_INPUT_INDEX = input_details[0]["index"]
        TFLITE_OUTPUT_DETAILS = output_details
        KERAS_MODEL = None
        return

    KERAS_MODEL = tf.keras.models.load_model(model_path, compile=False)
    TFLITE_INTERPRETER = None
    input_shape = KERAS_MODEL.input_shape
    if isinstance(input_shape, list):
        input_shape = input_shape[0]
    if len(input_shape) >= 3 and input_shape[1] and input_shape[2]:
        INPUT_HEIGHT = int(input_shape[1])
        INPUT_WIDTH = int(input_shape[2])


def _decode_image(image_base64: str) -> np.ndarray:
    image_bytes = base64.b64decode(image_base64)
    with Image.open(io.BytesIO(image_bytes)) as image:
        image = image.convert("RGB")
        image = image.resize((INPUT_WIDTH, INPUT_HEIGHT))
        array = np.asarray(image, dtype=np.float32)

    # Donkey/Keras models normally expect normalized RGB batches.
    array = array / 255.0
    return np.expand_dims(array, axis=0)


def _as_float(value: Any) -> float:
    array = np.asarray(value).reshape(-1)
    if array.size == 0:
        return 0.0
    return float(array[0])


def _parse_prediction(raw_prediction: Any) -> tuple[float, float]:
    if isinstance(raw_prediction, (list, tuple)):
        if len(raw_prediction) >= 2:
            return _as_float(raw_prediction[0]), _as_float(raw_prediction[1])
        return _as_float(raw_prediction[0]), 0.0

    flat = np.asarray(raw_prediction).reshape(-1)
    angle = float(flat[0]) if flat.size >= 1 else 0.0
    throttle = float(flat[1]) if flat.size >= 2 else 0.0
    return angle, throttle


def _predict(batch: np.ndarray) -> tuple[float, float]:
    if TFLITE_INTERPRETER is not None:
        TFLITE_INTERPRETER.set_tensor(TFLITE_INPUT_INDEX, batch.astype(np.float32))
        TFLITE_INTERPRETER.invoke()
        outputs = [
            TFLITE_INTERPRETER.get_tensor(detail["index"])
            for detail in TFLITE_OUTPUT_DETAILS
        ]
        return _parse_prediction(outputs)

    if KERAS_MODEL is None:
        raise RuntimeError("Model is not loaded")

    prediction = KERAS_MODEL.predict(batch, verbose=0)
    return _parse_prediction(prediction)


@app.get("/health")
def health():
    return jsonify(
        {
            "ok": KERAS_MODEL is not None or TFLITE_INTERPRETER is not None,
            "model": MODEL_PATH,
            "input_width": INPUT_WIDTH,
            "input_height": INPUT_HEIGHT,
        }
    )


@app.post("/predict")
def predict():
    payload = request.get_json(silent=True) or {}
    image_base64 = payload.get("image_base64") or payload.get("image")
    if not image_base64:
        return jsonify({"error": "image_base64 is required"}), 400

    try:
        batch = _decode_image(image_base64)
        angle, throttle = _predict(batch)
        return jsonify({"angle": angle, "throttle": throttle})
    except Exception as exc:
        return jsonify({"error": str(exc)}), 500


def main() -> None:
    parser = argparse.ArgumentParser(description="Donkey Car pilot inference API server")
    parser.add_argument("--model", required=True, help="Path to .h5, .keras, or .tflite model inside WSL")
    parser.add_argument("--host", default="0.0.0.0")
    parser.add_argument("--port", type=int, default=5000)
    args = parser.parse_args()

    _load_model(args.model)
    app.run(host=args.host, port=args.port, threaded=True)


if __name__ == "__main__":
    main()
