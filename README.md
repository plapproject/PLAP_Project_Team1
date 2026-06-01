# Donkeycar Data Manager

Donkeycar Data Manager는 DonkeyCar Tub 데이터를 불러와 프레임을 확인하고, 잘못된 데이터를 제외/복원한 뒤 학습용 데이터와 모델 학습을 관리하는 C# WinForms 기반 도구입니다.

## 실행 방법

1. Visual Studio에서 `TeamApp.sln` 또는 `TeamApp.csproj`를 엽니다.
2. NuGet 패키지를 복원합니다.
3. `F5` 또는 아래 명령으로 실행합니다.

```powershell
dotnet run --project .\TeamApp.csproj
```

## 주요 기능

- DonkeyCar Tub V2 폴더와 여러 `catalog_*.catalog` 파일 로드
- 프레임 이미지, 조향각, 스로틀, 모드, 시나리오 확인
- 조향각/스로틀/모드/시나리오 기준 필터링
- 선택 프레임 제외, 구간 제외, 구간 복원
- 제외 상태 저장과 다음 로드 시 자동 복원
- 원본을 훼손하지 않는 Clean 폴더 추출
- 조향각/스로틀 차트 표시
- 튜토리얼 안내
- WSL/conda 기반 DonkeyCar 학습 실행과 로그 표시

## 학습 실행 기본값

현재 학습 실행 탭은 사용자가 직접 설정해야 하는 값을 최소화하도록 구성되어 있습니다.

- 기본 Tub 경로: `C:\Users\user\Desktop\data`
- 기본 모델 저장 경로: `C:\Users\user\Desktop\study\pilot.keras`
- 기본 mycar 경로: `~/mycar`
- 기본 conda 환경명: `e2e_env`
- 기본 모델 종류: `linear`
- 기본 학습 횟수: `1`

앱은 WSL에서 `~/miniconda3/bin/conda run --no-capture-output -n e2e_env donkey train ...` 형식으로 DonkeyCar 공식 CLI를 실행합니다. 학습 횟수는 DonkeyCar CLI의 `--epochs` 옵션이 아니라 임시 config 파일의 `MAX_EPOCHS` 값으로 제어합니다.

## 폴더 구조

```text
TeamApp/
├─ Form1.cs              # 주요 화면 로직과 데이터 처리
├─ Form1.Designer.cs     # WinForms UI 배치 코드
├─ TeamApp.csproj        # 프로젝트 설정
├─ README.md             # 프로젝트 설명 문서
├─ bin/                  # 빌드 산출물
└─ obj/                  # 빌드 중간 산출물
```

## 사용 방법

1. `데이터 폴더 열기`로 DonkeyCar Tub 폴더를 선택합니다.
2. 프레임 표와 미리보기 이미지에서 데이터를 확인합니다.
3. 필터 조건을 입력하고 `필터 적용`으로 이상 후보를 찾습니다.
4. `선택 프레임 제외` 또는 `구간 제외`로 학습에서 제외할 데이터를 표시합니다.
5. 실수로 제외한 데이터는 `복원`으로 되돌립니다.
6. `상태 저장`으로 제외 상태를 저장합니다.
7. `클린 폴더 추출`로 학습에 사용할 Clean 데이터셋을 만듭니다.
8. `학습 실행` 탭에서 `학습 시작`을 누르면 기본 경로 기준으로 학습을 실행합니다.

## 에러 해결 방법

- Tub 폴더를 찾을 수 없다는 메시지가 나오면 `C:\Users\user\Desktop\data`에 `catalog_*.catalog`, `manifest.json`, `images` 폴더가 있는지 확인합니다.
- 학습 실행에서 conda 환경 오류가 나오면 WSL에서 `~/miniconda3/bin/conda env list`를 실행해 `e2e_env`가 있는지 확인합니다.
- `donkey` 명령을 찾을 수 없으면 WSL의 `e2e_env` 환경 안에 DonkeyCar가 설치되어 있는지 확인합니다.
- 모델 저장 오류가 나오면 `C:\Users\user\Desktop\study` 폴더 권한을 확인합니다.
- 빌드 중 `NU1701` 경고가 보일 수 있습니다. 현재 빌드는 가능하지만 패키지 호환성 경고이므로, 별도 요청이 있을 때만 프로젝트 타깃 프레임워크 설정을 조정합니다.
