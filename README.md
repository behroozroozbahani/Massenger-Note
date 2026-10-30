# Messenger Note

A backend API for a messaging and note-taking application, built with ASP.NET Core using Clean Architecture and CQRS. It provides secure JWT authentication, private and group messaging with file attachments, and personal notes, with a separate set of admin operations.

## Features

**Accounts and security**
- Register, login, logout, and refresh-token endpoints
- JWT bearer authentication with refresh tokens and configurable token lifetimes
- ASP.NET Core Identity with roles and policy-based authorization, including a dynamic authorize filter
- Account lockout, password-history checks, and a seeded admin user

**Messaging**
- Private messages between two users, with replies (parent message) and file attachments
- Group messages in messenger groups, with replies and attachments
- Create, update, and delete groups; add and remove group members
- Message listings by sender/recipient or by group, plus admin views of all messages
- Image attachments are validated, stored on disk, and get automatically generated thumbnails (ImageSharp). Original and thumbnail files are served through separate endpoints.

**Notes**
- Create, read, update, and delete notes with file attachments
- Separate user and admin operations for notes and note files

**Platform**
- Swagger UI with a bearer-token authorization filter
- Structured logging with Serilog (SQL Server sink) and background jobs with Hangfire
- Persian-aware text handling (Persian Ye/Ke normalization, Persian utilities)

## Architecture

```
Src
├── Domain           Entities (Note, MessengerGroup, messages, files, Identity), value objects, auditing and soft-delete interfaces
├── Application      Commands, queries, validators, DTOs, mapping profiles, MediatR pipeline
