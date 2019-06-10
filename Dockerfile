FROM mcr.microsoft.com/dotnet/core/sdk:2.2-alpine AS build
WORKDIR /build

COPY . ./
RUN dotnet publish CSharpAnalyser.App -c Release -o ../publish

FROM mcr.microsoft.com/dotnet/core/runtime:2.2-alpine
WORKDIR /app
COPY --from=build /build/publish .

RUN addgroup -S appGroup && \
    adduser -S appUser -G appGroup

USER appUser
ENTRYPOINT ["dotnet", "CSharpAnalyser.App.dll"]
