#!/bin/bash
set -e

# Skip Angular build since we're only deploying the backend
# cd cms.client
# npm install
# npm run build --configuration production

# Skip copying Angular files
# mkdir -p CMS.Server/wwwroot
# cp -r cms.client/dist/cms.client/* CMS.Server/wwwroot/

# Build .NET app only
cd CMS.Server
dotnet publish -c Release

# Note: For Neon DB, migrations should be run separately
# Not in the build script as you'll use an external DB