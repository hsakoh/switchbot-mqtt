ARG BUILD_FROM
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine as builder

WORKDIR /src
COPY /src .
RUN dotnet restore "SwitchBotMqttApp/SwitchBotMqttApp.csproj"
RUN dotnet publish "SwitchBotMqttApp/SwitchBotMqttApp.csproj" -r linux-musl-arm64 -p:PublishSingleFile=true --self-contained false -c Release -o /app/aarch64 --no-restore
RUN dotnet publish "SwitchBotMqttApp/SwitchBotMqttApp.csproj" -r linux-musl-x64 -p:PublishSingleFile=true --self-contained false -c Release -o /app/amd64 --no-restore
RUN dotnet publish "SwitchBotMqttApp/SwitchBotMqttApp.csproj" -r linux-musl-arm -p:PublishSingleFile=true --self-contained false -c Release -o /app/armv7 --no-restore

FROM $BUILD_FROM as final

ARG BUILD_ARCH
ENV BUILD_ARCH=$BUILD_ARCH

# Install requirements for add-on
RUN \
  apk add --no-cache \
    bash ca-certificates-bundle libgcc libssl3 libstdc++ zlib aspnetcore8-runtime

COPY --from=builder /app/$BUILD_ARCH /app
WORKDIR /app
ENTRYPOINT ["./SwitchBotMqttApp"]