# ASSLoader.NET
A .NET Library for ass subtitle file loading and writing.

# Update Logs

v0.9.1
---------------------
* When load ass file, unknown sections would be also stored to object.
  So the unknown sections would be write back to the file.
* Make StartTime and EndTime of each subtitle line to a managed type
  - `ASSEventTime`
