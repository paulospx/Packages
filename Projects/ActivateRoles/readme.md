In .NET (Core, 5, 6, 7, 8…), you can publish your project as a **single self-contained executable** using the `dotnet publish` command with the right options.

---

## **Command**

From your project folder:

```bash
dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true
```

### **Explanation**

* `-r win-x64` → Target runtime (change to `win-arm64`, `linux-x64`, `osx-x64`, etc.).
* `-c Release` → Use Release configuration.
* `/p:PublishSingleFile=true` → Bundle into one `.exe` (or single binary on Linux/Mac).

---

## **Optional Tweaks**

1. **Trim unused libraries** (smaller file size, might break reflection-heavy apps):

```bash
/p:PublishTrimmed=true
```

2. **No console window for GUI apps** (Windows only):

```bash
/p:WindowsGUI=true
```

3. **Embed the runtime** so the target machine doesn’t need .NET installed:

```bash
dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true /p:SelfContained=true
```

---

## **Example: Self-contained single .exe for Windows**

```bash
dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true /p:SelfContained=true /p:PublishTrimmed=true
```

Output will be in:

```
bin\Release\netX.X\win-x64\publish\
```

---

Do you want me to make you a **ready-to-compile C# PIM role activation tool** and package it as a single `.exe`?
That way you could just run it without needing to install .NET on the target machine.




{HTTP method} https://graph.microsoft.com/{version}/{resource}?{query-parameters}

/servicePrincipals/{id}

 roledefinitionid

 /servicePrincipals/{id}/appRoleAssignments