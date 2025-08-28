# Docker README

**Byg Docker image:**
```sh
docker build -t blazorwebapp:v1 ./H4.Day1.Identity
```

**Kør container med certifikat:**
```sh
docker run --name blazorwebapi1container -p 8000:80 -p 8001:443 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORT=8001 -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/MyBlazorApp.pfx -e ASPNETCORE_Kestrel__Certificates__Default__Password="Jensen@1" -e ASPNETCORE_ENVIRONMENT=Development -v ./H4.Day1.Identity/Cert:/https blazorwebapp:v1
```

#### Pro-tip
Når man vælger at tilføje Docker support, kan vælge at sætte "Container Build Context" til projektmappen, så du ikke behøver flytte din Dockerfile til solution folder.  
