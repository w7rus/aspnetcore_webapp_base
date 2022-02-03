#!/bin/sh
set -e

psql -v ON_ERROR_STOP=1 --username "postgres" --dbname "aspdb" <<-EOSQL
  create extension if not exists "hstore";
  select * FROM pg_extension;
EOSQL