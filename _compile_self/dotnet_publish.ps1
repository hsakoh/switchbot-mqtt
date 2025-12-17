dotnet restore "src/SwitchBotMqttApp/SwitchBotMqttApp.csproj"
dotnet clean "src/SwitchBotMqttApp/SwitchBotMqttApp.csproj" -c Release
dotnet publish "src/SwitchBotMqttApp/SwitchBotMqttApp.csproj" -r linux-musl-arm64 -p:PublishSingleFile=true --no-self-contained -c Release -o "./_compile_self/aarch64" --no-restore
dotnet publish "src/SwitchBotMqttApp/SwitchBotMqttApp.csproj" -r linux-musl-x64 -p:PublishSingleFile=true --no-self-contained -c Release -o "./_compile_self/amd64" --no-restore
dotnet publish "src/SwitchBotMqttApp/SwitchBotMqttApp.csproj" -r linux-musl-arm -p:PublishSingleFile=true --no-self-contained -c Release -o "./_compile_self/armv7" --no-restore
