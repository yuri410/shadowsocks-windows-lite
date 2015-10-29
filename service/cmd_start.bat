@echo off
for %%f in (sv_*.exe) do (

    echo installing %%~nf

    %%~nf start
)
pause