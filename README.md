# Mag

## Overview
Just a docker snippet for minimal .NET image (`runtime-deps:8.0-jammy-chiseled`) with `IHost` abstraction

### Configure
Edit [appsettings.toml](appsettings.toml) file then..

### Run
```bash
git clone --depth=1 https://github.com/dandriano/Mag.git && \
cd Mag && \
docker build . -t mag && \
docker run --rm -it -p 8080:8080 mag
```

## More
1. Problem [overview](https://habr.com/ru/companies/lamoda/articles/807179/)