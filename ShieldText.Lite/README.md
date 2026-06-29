# ShieldText Lite

> A lightweight text encryption tool built with WinForms and .NET Framework 4.8

![Platform](https://img.shields.io/badge/platform-Windows-blue)
![.NET](https://img.shields.io/badge/.NET_Framework-4.8-purple)
![License](https://img.shields.io/badge/license-MIT-green)
![Size](https://img.shields.io/badge/size-~15KB-brightgreen)

---

## Overview

ShieldText Lite is an ultra-lightweight Windows desktop application for encrypting and decrypting text. At just **~15KB**, it runs on any Windows machine without any installation or additional runtime — because .NET Framework 4.8 is already built into Windows.

---

## Features

- 🔒 **AES-256-CBC** encryption — industry-standard symmetric encryption
- 🔑 **PBKDF2** key derivation with SHA-1 — protects against brute-force attacks
- 🧂 **Random Salt + IV** per encryption — same text never produces the same output twice
- 👁 **Show / Hide password** toggle
- 📋 **Copy to clipboard** — one click to copy encrypted output
- 💾 **Export to file** — save output as `.txt`
- ⇄ **Swap** — move output back to input for quick decryption
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
  AES-256-CBC Encryption
  (Random Salt + Random IV per operation)
        │
        ▼
  Base64 Encoded Output
  [Salt (16B) + IV (16B) + CipherText]
```

---

## Tech Stack

| Layer | Technology |
|---|---|
| Language | C# |
| Framework | .NET Framework 4.8 |
| UI | WinForms |
| Encryption | AES-256-CBC |
| Key Derivation | PBKDF2 / RFC 2898 |

---

## Why So Small?

ShieldText Lite is **~15KB** because .NET Framework 4.8 ships pre-installed on all modern Windows systems (Windows 10 and later). The executable contains only the application code — no bundled runtime.

| Version | Size | Requires |
|---|---|---|
| ShieldText (WPF) | ~5MB+ | .NET 10 Runtime |
| ShieldText Lite | ~15KB | .NET Framework 4.8 (pre-installed) |

---

## Getting Started

### Download
Head to [Releases](https://github.com/yourusername/ShieldText/releases) and download `ShieldText.Lite.exe` — no installation required. Just double-click and run.

### Requirements
- Windows 10 or later
- .NET Framework 4.8 (pre-installed on Windows 10+)

### Run from source
```bash
git clone https://github.com/yourusername/ShieldText.git
cd ShieldText/ShieldText.Lite
```
Open `ShieldText.sln` in Visual Studio, set `ShieldText.Lite` as startup project, and run.

---

## Security Notes

- Passwords are **never stored** anywhere
- Each encryption uses a **unique random Salt and IV**
- Wrong password on decrypt throws a **CryptographicException** — no partial decryption possible

---

## Related

- [ShieldText](https://github.com/yourusername/ShieldText) — the full WPF version with AES-256-GCM and MVVM architecture

---

## License

MIT License — free to use, modify, and distribute.

---

<p align="center">Powered by Nader Asadpour</p>