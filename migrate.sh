#!/usr/bin/env bash

# download dotnet-install script and install dotnet sdk
curl -sSL https://dot.net/v1/dotnet-install.sh > dotnet-install.sh
chmod u+x dotnet-install.sh
./dotnet-install.sh --version 2.1.604

# migrate database
dotnet exec \
   --runtimeconfig ./slideshow.runtimeconfig.json \
   --depsfile ./slideshow.deps.json /root/.dotnet/sdk/2.1.604/DotnetTools/dotnet-ef/2.1.11/tools/netcoreapp2.1/any/tools/netcoreapp2.0/any/ef.dll \
   --verbose database update \
   --context PostgresSlideshowContext \
   --startup-assembly ./slideshow.dll \
   --assembly ./slideshow.db.postgres.dll \
   --data-dir ./