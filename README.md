# Creating the Containers

Create all the containers defined in `docker-compose.yml` using the following command.

```ps
docker-compose -f docker-compose.yml up -d
```

Only create the specific services e.g. `mssql mysql`

```ps
docker-compose -f docker-compose.yml up -d mssql mysql
```

In conjuction with the above commands, you can also supply the following.

- `--force-recreate` - Recreate containers even if their configuration and image haven't changed
- `--build` - Build images before starting containers
- `--remove-orphans` - Remove containers for services not defined in the Compose file

## mssql on ARM [Optional]

> Update: `azure-sql-edge` used to be the only option to run `mssql` on `arm` macs. Now, thanks to _x86_64/amd64 Rosetta emulation on Apple Silicon_, we can use the full version. [Read about it here](https://devblogs.microsoft.com/azure-sql/development-with-sql-in-containers-on-macos/). If you want though, you can still use `azure-sql-edge` using the details below.

The `azure-sql-edge` image which contains a special version of SQL Server designed for IoT/Edge computing which can run on `arm`. There are some [unsupported features](https://learn.microsoft.com/en-us/azure/azure-sql-edge/features#unsupported-features) to be aware of, but is good enough for dev.

```ps
docker-compose -f docker-compose.yml -f docker-compose-arm.yml up -d
```

# mssql

### Initialisation

After creating the container for the first time, you might want to set the following settings. [Reference 🔗](https://learn.microsoft.com/en-us/sql/linux/sql-server-linux-configure-mssql-conf)

**Disable Telemetry**

```bash
/opt/mssql/bin/mssql-conf set telemetry.customerfeedback false
```

**Don't Force Encryption**

```bash
/opt/mssql/bin/mssql-conf set network.forceencryption 0
```

**View Current Config**

```bash
cat /var/opt/mssql/mssql.conf
```
