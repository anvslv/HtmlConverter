@echo Delete bin folders
for /d /r . %%d in (bin) do @if exist "%%d" rd /s/q "%%d"
@echo

@echo Delete obj folders
for /d /r . %%d in (obj) do @if exist "%%d" rd /s/q "%%d"
@echo
