FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/Service.FeeShareEngine.Writer/Service.FeeShareEngine.Writer.csproj", "Service.FeeShareEngine.Writer/"]
RUN dotnet restore "src/Service.FeeShareEngine.Writer/Service.FeeShareEngine.Writer.csproj"
COPY . .
WORKDIR "/src/Service.FeeShareEngine.Writer"
RUN dotnet build "Service.FeeShareEngine.Writer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Service.FeeShareEngine.Writer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Service.FeeShareEngine.Writer.dll"]
