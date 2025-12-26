ARG BUILD_FROM
FROM $BUILD_FROM

ARG BUILD_ARCH
ENV BUILD_ARCH=$BUILD_ARCH

# Install requirements for add-on
RUN apk add --upgrade --no-cache bash ca-certificates-bundle libgcc libssl3 libstdc++ icu-libs

COPY /$BUILD_ARCH /app
WORKDIR /app
ENTRYPOINT ["./SwitchBotMqttApp"]
