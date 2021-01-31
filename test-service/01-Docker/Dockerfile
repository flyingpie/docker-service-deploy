FROM mcr.microsoft.com/dotnet/aspnet:3.1.9

COPY bin /app/bin

WORKDIR /app

EXPOSE 80

ENTRYPOINT ["dotnet", "bin/Flyingpie.DockerServiceDeploy.SampleAPI.dll"]

HEALTHCHECK CMD curl --fail http://localhost:80/health || exit
