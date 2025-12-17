ARG BUILD_FROM
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS builder

ARG BUILD_ARCH
ENV BUILD_ARCH=$BUILD_ARCH

WORKDIR /src
COPY /src .
RUN dotnet restore "SwitchBotMqttApp/SwitchBotMqttApp.csproj"
RUN \
  if [ "${BUILD_ARCH}" = "aarch64" ]; then \
      rid=linux-musl-arm64; \
  elif [ "${BUILD_ARCH}" = "amd64" ]; then \
      rid=linux-musl-x64; \
  else \
      rid=linux-musl-arm; \
  fi \
  && dotnet publish "SwitchBotMqttApp/SwitchBotMqttApp.csproj" -r $rid -p:PublishSingleFile=true --self-contained -c Release -o /app --no-restore

RUN ls -l /app

FROM $BUILD_FROM AS final

ARG BUILD_ARCH
ENV BUILD_ARCH=$BUILD_ARCH

# Install requirements for add-on
RUN apk add --upgrade --no-cache bash ca-certificates-bundle libgcc libssl3 libstdc++ icu-libs

COPY --from=builder /app /app
WORKDIR /app
ENTRYPOINT ["./SwitchBotMqttApp"]