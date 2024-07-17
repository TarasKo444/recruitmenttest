FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RecruitmentTest.Api/RecruitmentTest.Api.csproj", "RecruitmentTest.Api/"]
COPY ["RecruitmentTest.Application/RecruitmentTest.Application.csproj", "RecruitmentTest.Application/"]
COPY ["RecruitmentTest.Common/RecruitmentTest.Common.csproj", "RecruitmentTest.Common/"]
COPY ["RecruitmentTest.Infrastructure/RecruitmentTest.Infrastructure.csproj", "RecruitmentTest.Infrastructure/"]
RUN dotnet restore "RecruitmentTest.Api/RecruitmentTest.Api.csproj"
COPY . .
WORKDIR "/src/RecruitmentTest.Api"
RUN dotnet build "RecruitmentTest.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RecruitmentTest.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RecruitmentTest.Api.dll"]
