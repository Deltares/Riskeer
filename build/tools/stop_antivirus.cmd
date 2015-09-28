rem server is used to build source only in automated way

net stop "SavRoam" > nul 2>&1
net stop "Symantec AntiVirus" > nul 2>&1
net stop "Symantec AntiVirus Definition Watcher" > nul 2>&1
net stop "Symantec Event Manager" > nul 2>&1
net stop "Symantec Network Drivers Service" > nul 2>&1
net stop "Symantec Settings Manager" > nul 2>&1
net stop "Symantec SPBBCSvc" > nul 2>&1
net stop "Symantec Endpoing Protection" > nul 2>&1
net stop "Symantec Management Client" > nul 2>&1


net stop "Sophos Anti-Virus" > nul 2>&1
net stop "Sophos Agent" > nul 2>&1
net stop "Sophos AutoUpdate Service" > nul 2>&1
net stop "Sophos Anti-Virus status reporter" > nul 2>&1
net stop "Sophos AutoUpdate Service" > nul 2>&1
net stop "Sophos Message Router" > nul 2>&1

exit 0