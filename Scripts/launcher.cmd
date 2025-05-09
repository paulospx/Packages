@echo off

REM ============================================================================
REM Batch script to launch two applications in the background: initiator.exe
REM and worker.exe. This script ensures that both applications are started and
REM keeps the console clean by suppressing outputs.
REM ============================================================================

REM Enable delayed expansion for advanced variable handling.
setlocal enabledelayedexpansion

REM Define paths to the executables (if not in the same directory as this script).
set INITIATOR_PATH="C:\path\to\initiator.exe"
set WORKER_PATH="C:\path\to\worker.exe"

REM Start initiator.exe in the background.
echo Starting initiator.exe...
start "Initiator" /B !INITIATOR_PATH!
if errorlevel 1 (
    echo Failed to start initiator.exe. Please check the path and try again.
    goto :end
)

REM Start worker.exe in the background.
echo Starting worker.exe...
start "Worker" /B !WORKER_PATH!
if errorlevel 1 (
    echo Failed to start worker.exe. Please check the path and try again.
    goto :end
)

REM Inform the user that both applications have been started successfully.
echo Both applications have been launched successfully.

REM End of script.
:end
REM Optionally pause for debugging purposes.
REM pause
