# ASSLoader.NET
A .NET Library for ass subtitle file loading and writing.

# Update Logs

## v0.9.2 - 2021-05-14

* Divide the `ASSSubtitle.cs` into files by classes.
* Add some document comments.
* Add `DefaultFormat` for `ASSEvent` and `ASSStyle`.
* Add `CreateNew()` for `ASSSubtitle` to create new ass file with a default template with `Default` style and empty events.
* Fixed bugs by @no1d

TODO:

* Complete document comments.
* Full reference document.

v0.9.1 - 2020-05-14
---------------------
* When load ass file, unknown sections would be also stored to object.
  So the unknown sections would be write back to the file.
* Make StartTime and EndTime of each subtitle line to a managed type
  - `ASSEventTime`

# Contributor

<a href="https://github.com/toshiya14"><img src="https://avatars.githubusercontent.com/u/13333533?v=4" title="toshiya14" width="80" height="80"></a>&nbsp;<a href="https://github.com/no1d"><img src="https://avatars.githubusercontent.com/u/11740438?v=4" title="no1d" width="80" height="80"></a>

