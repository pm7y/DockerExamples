# ./sonarqube

Create a postgres db container and a SonarQube web app instance and allow them to talk to each other.

```bash
docker-compose -f docker-compose-sonarqube.yml up -d
```

Refer to [blog article](https://m7y.me/post/2024-05-15-sonarqube-with-docker-compose/) for detailed setup and explanation.

# Other Useful Link

https://hub.docker.com/_/sonarqube
https://hub.docker.com/r/sonarsource/sonar-scanner-cli

https://docs.sonarsource.com/sonarqube/latest/analyzing-source-code/scanners/sonarscanner-for-dotnet/
https://www.sonarsource.com/products/sonarlint/
https://www.sonarsource.com/products/sonarqube/downloads/
