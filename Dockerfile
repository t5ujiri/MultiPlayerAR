FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS sdk
COPY . ./workspace

RUN dotnet publish ./workspace/MultiPlayerAR.Server/MultiPlayerAR.Server.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/runtime:3.1
COPY --from=sdk /app .
ENTRYPOINT ["dotnet", "MultiPlayerAR.Server.dll"]

# Expose ports.
EXPOSE 12345