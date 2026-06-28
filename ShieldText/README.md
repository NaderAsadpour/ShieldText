# 🛡 ShieldText

> Secure text encryption desktop application built with WPF and .NET 10

![Platform](https://img.shields.io/badge/platform-Windows-blue)
![.NET](https://img.shields.io/badge/.NET-10.0-purple)
![License](https://img.shields.io/badge/license-MIT-green)
![Architecture](https://img.shields.io/badge/architecture-MVVM-orange)

---

## Overview

ShieldText is a lightweight Windows desktop application for encrypting and decrypting text using industry-standard cryptographic algorithms. Designed with simplicity in mind — paste your text, enter a password, and get encrypted output instantly.

---

## Features

- 🔒 **AES-256-GCM** encryption — the same standard used in military and banking systems
- 🔑 **PBKDF2** key derivation with SHA-256 — protects against brute-force attacks
- 🧂 **Random Salt + IV** per encryption — same text never produces the same output twice
- 👁 **Show / Hide password** toggle
- 📋 **Copy to clipboard** — one click to copy encrypted output
- 💾 **Export to file** — save output as `.txt`
- ⇅ **Swap** — move output back to input for quick decryption
- 🗑 **Clear All** — wipe everything at once
- ✅ **Status feedback** — real-time success / error messages

---

## How It Works

```
Plain Text + Password
        │
        ▼
  PBKDF2 (100,000 iterations) → 256-bit Key
        │
        ▼
  AES-256-GCM Encryption
  (Random Salt + Random IV per operation)
        │
        ▼
  Base64 Encoded Output
  [Salt (16B) + IV (12B) + Tag (16B) + CipherText]
```

The encrypted output contains everything needed for decryption — no metadata stored separately.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Language | C# 13 |
| Framework | .NET 10 / WPF |
| Architecture | MVVM |
| Encryption | AES-256-GCM (`System.Security.Cryptography`) |
| Key Derivation | PBKDF2 / RFC 2898 |
| UI Pattern | Data Binding + INotifyPropertyChanged |

---

## Project Structure

```
ShieldText/
├── Services/
│   └── CryptoService.cs       # AES-256-GCM encrypt/decrypt logic
├── ViewModels/
│   └── MainViewModel.cs       # MVVM ViewModel, state management
├── Helpers/
├── MainWindow.xaml            # UI layout
├── MainWindow.xaml.cs         # Code-behind, event handlers
└── App.xaml
```

---

## Getting Started

### Prerequisites
- Windows 10 or later
- [.NET 10 Runtime](https://dotnet.microsoft.com/download/dotnet/10.0)

### Run from source
```bash
git clone https://github.com/NaderAsadpour/ShieldText.git
cd ShieldText
dotnet run
```

### Or download
Head to [Releases](https://github.com/NaderAsadpour/ShieldText/releases) and download the latest `ShieldText.exe` — no installation required.

---

## Security Notes

- Passwords are **never stored** anywhere
- Each encryption uses a **unique random Salt and IV** — encrypting the same text twice produces different output
- The **GCM authentication tag** ensures encrypted data has not been tampered with
- Wrong password on decrypt throws an `AuthenticationTagMismatchException` — no partial decryption possible

---

## License

MIT License — free to use, modify, and distribute.

---

<p align="center">Powered by Nader Asadpour</p>