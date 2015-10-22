#
# Programmer Jan Mooiman
# Date       9 July 2014
#
import os              #  file exists
import sys             #  system
import subprocess      #  needed to run a subprocess and catch the result
import shutil          #  shell utilities, needed for file copy and removing directories
import datetime as dt  #  for timings


_initexmf = 'not set'
_miktexpm = 'not set'
_pdflatex = 'not set'
_svnexe = 'not set'


def remove_file(root, file_name):
    filename = os.path.join(root, file_name)
    if os.path.isfile(filename):
        os.remove(filename)


def remove_dir(root, dir_name):
    dir = os.path.join(root, dir_name)
    if os.path.isdir(dir):
        shutil.rmtree(dir)


def is_exe(fpath):
    return os.path.isfile(fpath) and os.access(fpath, os.X_OK)


def which(program):
    fpath, fname = os.path.split(program)
    if fpath:
        if is_exe(program):
            return program
    else:
        for path in os.environ["PATH"].split(os.pathsep):
            path = path.strip('"')
            exe_file = os.path.join(path, program)
            if is_exe(exe_file):
                return exe_file
    return None


def check_installation():
    global _initexmf
    global _miktexpm
    global _pdflatex
    global _svnexe
    error = 0
    
    _initexmf = which('initexmf.exe')
    _miktexpm = which('mpm.exe')

    print 'Using initexmf: %s' % _initexmf
    print 'Using mpm     : %s' % _miktexpm

    if (_initexmf is None or
        _miktexpm is None):
            error = 1

    _svnexe = which('svn.exe')
    if _svnexe is None:
        _svnexe = 'not found'
        error = 1
    print('Using svn      : %s' % _svnexe)

    if error == 1:
        print('Not all needed executables are found')
        sys.exit(1)

    print('')


def main(argv):
    global _initexmf, _miktexpm, _pdflatex, _svnexe
    
    start_time = dt.datetime.now().strftime('%H:%M:%S.%f')

    check_installation()

    repository = 'https://repos.deltares.nl/repos/DeltaresStyles/trunk/style_guide_and_documents/templates/latex'
    appdatamiktekroot = os.environ["APPDATA"] + "/MiKTeX/2.9/"

    remove_file(appdatamiktekroot, 'makeindex/nomentbl/deltares_nomentbl.ist')
    remove_file(appdatamiktekroot, 'makeindex/nomentbl/deltares_nomentbl_nl.ist')
    remove_file(appdatamiktekroot, 'bibtex/bst/misc/deltares_chicago_like.bst')
    remove_file(appdatamiktekroot, 'bibtex/bst/misc/deltares_chicago_like_dutch.bst')
    remove_file(appdatamiktekroot, 'bibtex/bst/misc/deltares_chicago_like_spanish.bst')

    remove_dir(appdatamiktekroot, 'templates')
    remove_dir(appdatamiktekroot, 'tex/latex/misc')

    print('Repository: %s' % repository)
    print('MiKTeXuser: %s' % appdatamiktekroot)

    svn_execute = '%s export --username "dscguest" --password "svngu3st" --no-auth-cache --non-interactive --trust-server-cert --force "%s" "%s"' % (_svnexe, repository, appdatamiktekroot)
    # print(svn_execute)
    out = subprocess.Popen(svn_execute, stdout=subprocess.PIPE, stderr=subprocess.STDOUT).communicate()[0]
    print(out.decode("utf-8"))

    print(_initexmf)
    out = subprocess.Popen([_initexmf, '--update-fndb'], stdout=subprocess.PIPE, stderr=subprocess.STDOUT).communicate()[0]
    print(out.decode("utf-8"))

    print(_miktexpm)
    out = subprocess.Popen([_miktexpm, '--update-db'], stdout=subprocess.PIPE, stderr=subprocess.STDOUT).communicate()[0]
    print(out.decode("utf-8"))

    print('\nStart: %s' % start_time)
    print('End  : %s' % dt.datetime.now().strftime('%H:%M:%S.%f'))
    print('Done')


if __name__ == "__main__":
    main(sys.argv[0:])