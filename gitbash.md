**posh-git** is a PowerShell-specific module and does **not** run natively in **Git Bash** (which is a bash shell via MinGW/MSYS2 on Windows 10).

You have two good paths on Windows 10 in 2026:

1. Use **oh-my-posh** (highly recommended) — modern, actively maintained, cross-shell, beautiful Git status + icons + many themes.
2. Use a bash port called **posh-git-sh** (older, simpler, closer to classic posh-git look but no icons).

Most people today choose **oh-my-posh** in Git Bash — it's what gives the closest (and actually better) experience to posh-git.

### Recommended: Install oh-my-posh in Git Bash on Windows 10

#### Step 1 – Install oh-my-posh (easiest with winget or scoop)

Open **PowerShell** (or **cmd**) as normal user and run one of these:

- Using **winget** (built-in on Windows 10 1809+):
  ```
  winget install JanDeDobbeleer.OhMyPosh -s winget
  ```

- Using **scoop** (very popular & clean):
  First install scoop if you don't have it:
  ```
  iwr -useb get.scoop.sh | iex
  ```
  Then:
  ```
  scoop install https://github.com/JanDeDobbeleer/oh-my-posh/releases/latest/download/oh-my-posh.json
  ```

After install → close & reopen Git Bash so PATH updates.

Verify:
```bash
oh-my-posh --version
```
(Should print a version number.)

#### Step 2 – Install a Nerd Font (required for icons & nice look)

1. Download a Nerd Font, e.g.:
   - **CaskaydiaCove Nerd Font** (very popular): https://www.nerdfonts.com/font-downloads → search "Cascadia Code"
   - Or **FiraCode Nerd Font**, **MesloLGS NF**, **JetBrainsMono NF**

2. Install the font (double-click .ttf files → Install).

3. In **Git Bash** window (or better: Windows Terminal → Git Bash profile):
   - Right-click title bar → Properties → Font tab → choose your new Nerd Font.

(If using **Windows Terminal**, go to Settings → Profiles → Git Bash → Appearance → Font face.)

#### Step 3 – Configure Git Bash to use oh-my-posh

Git Bash usually loads `~/.bash_profile` (not always `~/.bashrc`).

1. Open or create the file:
   ```bash
   notepad ~/.bash_profile
   ```
   (or use nano/vim if you prefer: `nano ~/.bash_profile`)

2. Add this at the end (or replace any existing oh-my-posh line):
   ```bash
   # Source .bashrc if it exists (optional but good practice)
   if [ -f ~/.bashrc ]; then
       . ~/.bashrc
   fi

   # Oh My Posh - pick ONE theme you like (examples below)
   eval "$(oh-my-posh init bash --config "$HOME/.poshthemes/jandedobbeleer.omp.json")"
   # or other nice ones:
   # eval "$(oh-my-posh init bash --config "$HOME/.poshthemes/amro.omp.json")"          # very posh-git like
   # eval "$(oh-my-posh init bash --config "$HOME/.poshthemes/clean-detailed.omp.json")"
   # eval "$(oh-my-posh init bash --config "$HOME/.poshthemes/agnoster.omp.json")"
   ```

   **Where are the themes?**
   - After install they are usually in: `C:\Users\YourUser\AppData\Local\oh-my-posh\themes` (Windows path)
   - In Git Bash → `$HOME/.poshthemes/` or `$POSH_THEMES_PATH` (often auto-set)
   - Browse themes & previews: https://ohmyposh.dev/docs/themes

3. Save and reload Git Bash:
   ```bash
   source ~/.bash_profile
   ```
   Or just close & reopen Git Bash.

You should now see a colored Git-aware prompt with branch, status icons, etc.

#### Quick troubleshooting
- `oh-my-posh: command not found` → check PATH: `which oh-my-posh` or reinstall with scoop/winget
- No icons → wrong font (must be Nerd Font) + restart terminal
- Slow? → Rare in 2026 versions, but exclude oh-my-posh.exe from Windows Defender real-time scan if needed

### Alternative: Classic posh-git style via posh-git-sh (no icons, simpler)

1. Download the script:
   ```
   curl -o ~/git-prompt.sh https://raw.githubusercontent.com/lyze/posh-git-sh/master/git-prompt.sh
   ```

2. Edit `~/.bash_profile`:
   ```bash
   source ~/git-prompt.sh

   # Simple version
   PS1='[\u@\h \W$(__posh_git_ps1 " (%s)")]\$ '
   ```

3. Reload:
   ```bash
   source ~/.bash_profile
   ```

This gives branch + dirty/clean indicators — similar to old posh-git — but without fancy colors/icons.

**Bottom line for Windows 10 Git Bash in 2026**  
→ Go with **oh-my-posh** → prettier, faster development, more features.  
If you run into any error during setup, paste it here and I'll help debug! 🚀
