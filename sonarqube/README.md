# ./sonarqube

Create a postgres db container and a sonarqube web app instance and allow them to talk to each other.

```bash
docker-compose -f docker-compose-sonarqube.yml up -d
```

## Setup

**Login and set new password**

- Browse to `http://localhost:9000/`
- Login using defaults of `admin` / `admin`
- Set a new password

**Create an Auth token**

- Browse to `http://localhost:9000/account/security`
- Generate a new token
  ![Generate Token](./sonarqube/generate_sq_token.png)
- You can either create a _global analysis token_ (that can be used for any project) or a _user token_ that can be used for specific project or projects.
- Take note of the token, you won't be able to access it again

**Create a project (local)**

- Browse to `http://localhost:9000/projects/create?mode=manual`
- Enter the project details, including a project key
  ![Create Project](./sonarqube/create_sq_project.png)

**Ensure Oracle Java jdk17 is installed**

SonarQube seems to be quiet particular about it being jdk17.

- On MacOS (view homebrew)
  - `brew install openjdk@17`
  - `sudo ln -sfn /opt/homebrew/opt/openjdk@17/libexec/openjdk.jdk /Library/Java/JavaVirtualMachines/openjdk-17.jdk`
- On Windows
  - `winget install -e --id Oracle.JDK.17`
  - Ensure `java --version` returns 17
  - If you encounter java related errors, check the system environment variables
    - Ensure JDK_HOME is set to the new jdk17 path
    - Ensure that the PATH variable doesn't contain any other java installs

**Analyse a project (dotnet scanner)**

- Install (or update) the SonarQube dotnet scanner
  - `dotnet tool update --global dotnet-sonarscanner`
- Begin the analysis
  - `dotnet sonarscanner begin /k:"<project-key>" /d:sonar.token="<sonar-token>" /d:sonar.host.url="http://localhost:9000"`
- Build the project
  - `dotnet build ./src/<solution-name>.sln --no-incremental`
- End the analysis
  - `dotnet sonarscanner end /d:sonar.token="<sonar-token>"`

---

# Notes and Stuff

token: sqa_42f6984c4ed5636de3f694d8326bf02890367d15
https://github.com/microsoft/AspNetCore-React-WebApp

https://www.sonarsource.com/products/sonarlint/

https://www.sonarsource.com/products/sonarqube/downloads/

https://hub.docker.com/_/sonarqube
https://hub.docker.com/r/sonarsource/sonar-scanner-cli
https://docs.sonarsource.com/sonarcloud/advanced-setup/ci-based-analysis/overview-of-integrated-cis/
https://docs.sonarsource.com/sonarqube/latest/analyzing-source-code/scanners/sonarscanner-for-dotnet/
https://docs.sonarsource.com/sonarcloud/advanced-setup/monorepo-support/

https://docs.sonarsource.com/sonarcloud/advanced-setup/analysis-parameters/
https://docs.sonarsource.com/sonarcloud/advanced-setup/analysis-scope/

https://www.continuousimprover.com/2021/02/sonarqube-mixed-projects.html
https://community.sonarsource.com/t/monorepo-and-sonarqube/37990
https://github.com/mjstealey/sonarqube-ce-docker


```bash
docker run --rm \
  -e SONAR_HOST_URL="http://host.docker.internal:9000" \
  -e SONAR_TOKEN="sqa_42f6984c4ed5636de3f694d8326bf02890367d15" \
  -e SONAR_SCANNER_OPTS="-Xmx512m -Dsonar.projectKey=AspNetCore-React-WebApp-FE -Dsonar.verbose=true -Dsonar.scm.provider=git -Dsonar.scm.disabled=false -Dsonar.projectBaseDir=. -Dsonar.sources=./client -Dsonar.exclusions=./client/**/*.test.tsx -Dsonar.tests=./client -Dsonar.test.inclusions=./client/**/*.test.tsx" \
  -v "/Users/paul.mcilreavy/src/AspNetCore-React-WebApp:/usr/src" \

  sonarsource/sonar-scanner-cli:latest;
```

# save some time above by mounting the cache folder
#  -v "./data/scanner-cache:/opt/sonar-scanner/.sonar/cache" \

#

```bash
dotnet tool update --global dotnet-sonarscanner;
dotnet add package coverlet.collector; # need this since --collect "Code Coverage"; is only supported on windows

dotnet sonarscanner begin \
  /k:"AspNetCore-React-WebApp-BE" \
  /d:sonar.token="sqa_42f6984c4ed5636de3f694d8326bf02890367d15" \
  /d:sonar.host.url="http://localhost:9000" \
  /d:sonar.verbose=true \
  /d:sonar.scm.provider=git \
  /d:sonar.scm.disabled=false \
  /d:sonar.projectBaseDir=. \
  /d:sonar.cs.opencover.reportsPaths="./**/coverage.opencover.xml";

dotnet build ./service -c release -v minimal --nologo --no-incremental;
dotnet test ./service -v minimal --no-build --results-directory "./service/TestResults" --collect "XPlat Code Coverage;Format=Cobertura,OpenCover";
dotnet sonarscanner end /d:sonar.token="sqa_42f6984c4ed5636de3f694d8326bf02890367d15";

rm -rf **/TestResults;rm -rf **/bin;rm -rf **/obj;

rm -rf **/TestResults **/bin **/obj;


# dev tunnel

https://learn.microsoft.com/en-us/azure/developer/dev-tunnels/get-started

install
- windows `winget install Microsoft.devtunnel`
  - close and reopen terminal
- macos `brew install --cask devtunnel`

devtunnel list
devtunnel delete-all

devtunnel create -a -e 1h pm7y-ss
devtunnel port create -p 9000 pm7y-ss
devtunnel host pm7y-ss
