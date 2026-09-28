# ---------- Stage 1: base (runtime) ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# ---------- Stage 2: build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia só os .csproj para aproveitar o cache do restore
COPY ["UsersAPI/UsersAPI.csproj", "UsersAPI/"]
COPY ["Infra/Infra.csproj", "Infra/"]
COPY ["Core/Core.csproj", "Core/"]
RUN dotnet restore "UsersAPI/UsersAPI.csproj"

# Copia o restante do código
COPY . .

# ---------- Stage 3: publish ----------
FROM build AS publish
RUN dotnet publish "UsersAPI/UsersAPI.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

# ---------- Stage 4: final ----------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER $APP_UID
ENTRYPOINT ["dotnet", "UsersAPI.dll"]