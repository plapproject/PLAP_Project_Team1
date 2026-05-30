# Donkeycar Data Manager

Donkeycar Data Manager는 DonkeyCar Tub 데이터를 불러와 프레임을 확인하고, 잘못된 데이터를 제외/복원한 뒤 학습용 Clean 폴더로 추출하는 WinForms 기반 데이터 관리 도구입니다.

## 실행 방법

1. Visual Studio에서 `TeamApp.sln` 또는 `TeamApp.csproj`를 엽니다.
2. NuGet 패키지 복원을 진행합니다.
3. `F5` 또는 `dotnet run`으로 실행합니다.

```powershell
dotnet run --project .\TeamApp.csproj
```

## 주요 기능

- DonkeyCar Tub 폴더 및 다중 `catalog_*.catalog` 로드
- 프레임 이미지, 조향각, 스로틀, 모드, 시나리오 확인
- 조향각/스로틀/모드/시나리오 기준 필터링
- 선택 프레임 제외, 구간 제외, 구간 복원
- 제외 상태 저장 및 다음 로드 시 자동 복원
- 원본을 보존한 학습용 `_Clean` 폴더 추출
- 조향각/스로틀 그래프 및 필터 상태별 차트 표시
- WSL/conda 기반 DonkeyCar 학습 실행 및 로그 관찰
- 기능별 튜토리얼과 첫 사용자 안내

## 폴더 구조

```text
TeamApp/
├─ Form1.cs              # 주요 화면 로직과 데이터 처리
├─ Form1.Designer.cs     # WinForms UI 배치 코드
├─ TeamApp.csproj        # 프로젝트 설정
├─ README.md             # 프로젝트 설명 문서
└─ bin/, obj/            # 빌드 산출물
```

## 사용 방법

1. `데이터 폴더 열기`로 Tub 폴더를 선택합니다.
2. 프레임 표에서 이미지명, 조향각, 스로틀, 상태를 확인합니다.
3. 필터 조건을 입력하고 `필터 적용`을 눌러 이상 후보를 좁힙니다.
4. `선택 프레임 제외` 또는 `구간 제외`로 학습에서 제외할 데이터를 표시합니다.
5. 실수로 제외한 데이터는 `복원`으로 되돌립니다.
6. `상태 저장`으로 제외 상태를 저장합니다.
7. `클린 폴더 추출`로 학습에 사용할 `_Clean` 폴더를 만듭니다.
8. `학습 실행` 탭에서 mycar 경로, Tub 경로, 모델 저장 경로, 모델 종류, Python 환경명을 입력한 뒤 학습을 실행합니다.

## 에러 해결 방법

- Tub 폴더가 비어 있으면 `catalog_*.catalog` 또는 이미지 파일이 있는 폴더인지 확인합니다.
- 필터 범위 오류가 나면 최소값이 최대값보다 작거나 같은지 확인합니다.
- 이미지가 표시되지 않으면 catalog의 이미지명과 실제 이미지 파일명이 일치하는지 확인합니다.
- 학습 실행 오류가 나면 WSL 설치 여부, `~/miniconda3/bin/conda`, Python 환경명, `mycar` 경로를 확인합니다.
- 빌드 시 `NU1701` 경고가 보일 수 있습니다. 현재 빌드는 가능하지만, 패키지 호환성 점검이 필요할 수 있습니다.
