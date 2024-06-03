
dotnet build -c Release
cd .\Build
move .\Build\docker-compose.yaml .\bin\Release\net6.0
move .\Build\dockerfile .\bin\Release\net6.0
move .\Build\.env .\bin\Release\net6.0
docker-compose up -d --build
docker-compose push
