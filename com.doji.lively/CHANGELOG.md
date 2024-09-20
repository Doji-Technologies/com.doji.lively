# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.2.0] - 2024-09-20

### Fixed

- Fixed bug that would result in trying to connect to an invalid endpoint and the session initiation to fail.

### Changed

- Refactored the API for streaming sessions. They are now non-MonoBehaviour classes.
- You now also have the option to stream from a Texture in addition to streaming from a Camera like before.

## [0.1.2] - 2024-08-22

### Fixed

- Found workaround to make it works on Windows platform

## [0.1.1] - 2023-06-24

### Fixed

- Fixed incorrect stream key validation

## [0.1.0] - 2023-06-24

- Initial Release