FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build
WORKDIR /source

# Copy only the project file first for caching purposes
COPY Mag.csproj ./
RUN dotnet restore -a x64 --verbosity diag

# Now copy the rest of the source code
COPY . .

# Publish the application
RUN dotnet publish -c Release -a x64 -p:PublishTrimmed=true --self-contained -o /app --no-restore

# Chiseled runtime image
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-jammy-chiseled AS runtime
USER app
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["./Mag"]
