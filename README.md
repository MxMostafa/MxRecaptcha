# MxCaptcha

A configurable CAPTCHA library for **ASP.NET (.NET 10)** and **ASP.NET (.NET Framework 4.5)** with image generation, validation, and optional cache/Redis-backed storage.

`MxCaptcha` is designed to be simple to integrate while still offering enough flexibility for production use.  
It supports customizable CAPTCHA generation, multiple rendering options, expiration control, and demo UIs for quick testing.

---

## Features

- Supports **.NET 10**
- Supports **.NET Framework 4.5**
- CAPTCHA image generation
- Configurable text rules:
  - numbers
  - letters
  - case sensitivity
  - code length
- Configurable image rendering:
  - width / height
  - font family / font size
  - random text colors
  - random rotation
  - wave distortion
  - noise level
  - character spacing
- Configurable expiration time
- Supports cache-based storage
- Supports Redis-based storage
- Includes **2 sample test projects**
- Includes a simple test UI: `captcha-test.html`

---


Sample Projects
This repository includes two sample applications:

MxCaptcha.AspNet → for .NET 10
MxCaptcha.AspNet-45 → for .NET Framework 4.5
Both samples include a simple UI that allows you to:

generate CAPTCHA images
display them in the browser
test validation behavior
Test UI
A lightweight UI is included for manual testing:

captcha-test.html
By running the sample project and opening this page, you can view the generated CAPTCHA and test the flow interactively.

## Project Structure
```text
MxCaptcha/
├── MxCaptcha.AspNet/          # Sample project for .NET 10
├── MxCaptcha.AspNet-45/       # Sample project for .NET Framework 4.5
├── captcha-test.html          # Simple UI to test captcha generation/validation
└── src / library code         # Main captcha library source





