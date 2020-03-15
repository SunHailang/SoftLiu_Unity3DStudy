#!/usr/bin/python
import sys
import os
import errno
import shutil
import subprocess
#Copyright (c) 2015 Future Games of London and Ubisoft
#Authored by Peter Pearson
#If python was a snake, I'd still like it. Probably not as much though.

#runs the gradle, dependant on platform, cus we're clever

#	BEGINING OF THE METHOD TO MAKE ZIE BUILDS
def makeZieBuildPlease(BUILD_TYPE, TYPE = "assemble"):
	print "Build method starting... Environment: " + BUILD_TYPE + " Type: " + TYPE 
	
	if BUILD_TYPE != "Debug" and BUILD_TYPE != "Release" :
		print "Bad Param. Try again with 'python gradle_build.py [Debug/Release]"
		return -1
	
	print(sys.platform)
		
	path = os.path.dirname(os.path.realpath(__file__))
	realPath = path;
	realPath = realPath.replace("/cygdrive/c", "C:"); #assumes we only use cygwin on pc
	realPath = realPath.replace("/cygdrive/d", "D:");
	realPath = realPath.replace("/cygdrive/e", "E:");
	realPath = realPath.replace("/cygdrive/f", "F:");
	os.chdir(realPath)

	executable = []
	if sys.platform == "windows" or sys.platform == "win32" or sys.platform == "win64":
		executable = ["java", "-Dorg.gradle.appname=gradlew", "-classpath", realPath+"\gradle\wrapper\gradle-wrapper.jar", "org.gradle.wrapper.GradleWrapperMain", TYPE+BUILD_TYPE, "--debug", "--stacktrace"]
	else:
		executable = ["java \"-Dorg.gradle.appname=gradlew\" -classpath \"" + realPath + "/gradle/wrapper/gradle-wrapper.jar\" org.gradle.wrapper.GradleWrapperMain " + TYPE+BUILD_TYPE + " --debug --stacktrace", ""]
		
	print executable
	proc = subprocess.Popen(executable, stdout=subprocess.PIPE, stderr=subprocess.PIPE, shell=True)
	(out, err) = proc.communicate()

	if os.path.exists(path+'/../gradle.log'):
		os.remove(path+'/../gradle.log')
	log = open(path+'/../gradle.log', 'w')
	log.write(out)
	log.close()

	print "Build method has completed."

	if err:
		if os.path.exists(path+'/../gradle_error.log'):
			os.remove(path+'/../gradle_error.log')
		errlog = open(path+'/../gradle_error.log', 'w')
		print "\n-------------------BUILD FAILED WITH ERRORS-------------------\n"
		print err
		print "\n-------------------BUILD FAILED WITH ERRORS-------------------\n"
		errlog.write("-------------------BUILD FAILED WITH ERRORS-------------------\n")
		errlog.write(err)
		errlog.close()
		return -1
	return 0
#	END OF THE METHOD OF ZIE BUILDS	

#PARAMS
#1: ScriptName - not sure why python does that.
#2: BuildType [Release/Debug]
if len(sys.argv) != 2:
	print "Overall method count: Too few/many arguments. Try again with 'python gradle_build.py [Debug/Release]"
	sys.exit(0);

BUILD_TYPE = sys.argv[1]
if BUILD_TYPE != "Debug" and BUILD_TYPE != "Release" :
	print "Bad Param. Try again with 'python gradle_build.py [Debug/Release]"
	sys.exit(0);
		
print(sys.platform)
	
#	Execute the method
returnValue = makeZieBuildPlease(BUILD_TYPE)

print "Return value of assemble build: " + str(returnValue)

if returnValue == 0 :
	print "Assemble Finished. Making bundle"
	returnValue = makeZieBuildPlease(BUILD_TYPE, "bundle")
	print "Return value of bundle build: " + str(returnValue)

