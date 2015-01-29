from ftplib import FTP
from datetime import datetime
from time import sleep
import subprocess
import os

ip = '193.0.159.183'
port = 8821
user = 'MagneV'
password = 'T33y6eG8vz'

if not os.path.exists('.git'):
    with open('.gitignore', 'w') as filehandle:
        filehandle.write('*.py')
    subprocess.call(['git', 'init'])


while 1:
    ftp = FTP()
    ftp.connect(ip, port)
    ftp.login(user, password)
    ftp.cwd(ftp.nlst()[0] + '/Saves/Lone Survivor 2014-09-22 2259')

    if not os.path.exists('save'):
        os.makedirs('save')

    for filename in ftp.nlst():
        print "Downloading", filename
        with open('save/' + filename, 'wb') as filehandle:
            ftp.retrbinary('RETR %s' % filename, filehandle.write)
        print "Finished", filename

    print "Creating git commit"
    subprocess.call(["git", "add", "."])
    subprocess.call(
        ["git", "commit", "-m", "'%s space engineeres backup'" % str(datetime.now())],
    )
    print "done"
    ftp.quit()
    print 'sleeping 10 minutes'
    sleep(10 * 60)
