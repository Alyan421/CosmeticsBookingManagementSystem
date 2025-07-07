FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app
COPY CMS.Server/bin/Release/net8.0/publish/ .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "CMS.Server.dll"]