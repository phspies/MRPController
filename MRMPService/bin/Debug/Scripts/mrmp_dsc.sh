#  (c) 2016 Dimension Data, All rights reserved.
# **************************************************************************
#  Module:  UNIX MRMP Discovery Script
#  Desc:    This script runs all the UNIX/Linux utilities to collect inventory
# **************************************************************************


# **************************************************************************
#  Control Section
# **************************************************************************
test_tools()
{
	 check=0
       TMP=`which $cmd 2>/dev/null`
	 if [ -x "$TMP" ] ; then
    	     IMPL=`$cmd 1>tmpout_DIUBS_DSIJB 2>tmperr_DSOND_SOBNS`
	     FOUND=`grep "not implemented" tmperr_DSOND_SOBNS`
	     UNREG=`grep "not registered" tmperr_DSOND_SOBNS`	
	 fi
     	 if [ -x "$TMP" ] && [ -z "$FOUND" ] && [ -z "$UNREG" ] ; then
	     check=1
       fi
	 `rm -rf tmperr_DSOND_SOBNS`
	 `rm -rf tmpout_DIUBS_DSIJB`
}

do_vm_detection()
{
	# Only Linux and Solaris OS is supported as VM
	case "$OS_TYPE" in
	  Linux)
			if [ ! -e /proc/scsi/scsi ] || [ -r /proc/vmware/version ] ; then
				echo ISRV_VM=Physical
			else
				cmd="sed"
				test_tools
				if [ "$check" -eq 0 ] ; then
					echo ISRV_VM=Unknown
				else
					( cat /proc/scsi/scsi
					echo Host: zzDummy Channel: 00 Id: 00 Lun: 00
					) |
					( # Read the "Attached devices:" line
					read LINE
					while read LINE ; do
						if expr "$LINE" : Vendor >/dev/null ; then
							VENDOR=`echo "$LINE" | sed -e 's/Vendor: *//' -e 's/ *Model.*//'`
							MODEL=`echo "$LINE" | cut -d: -f3 | sed -e 's/^ *//' -e 's/ *Rev.*//'`
							if [ "$VENDOR" = "MRMP" ] && [ "$MODEL" = "Virtual disk" ] ; then
								ISRV_VM=MRMP
								break
							fi
						fi
					done
					if [ -z "$ISRV_VM" ] ; then
						echo ISRV_VM=Physical
					else
						echo ISRV_VM=$ISRV_VM
					fi
					)
				fi
			fi
			;;
	  SunOS)
			# Solaris VM investigation incomplete
			echo ISRV_VM=Unknown
			;;
	  *)
			echo ISRV_VM=Physical
			;;
	esac
}

do_control()
{
	TMPDMESG=$MYTMPDIR/dmesginfo.$$
	OS_TYPE=`uname -s`
	CPU_TYPE=`uname -m`

	echo '<CONTROL>'
	echo VERSION=100
	echo HOSTNAME=`uname -n`
	echo FILETYPE=Inv
	echo DATE=`date`
	echo TIMESTAMP=`date '+%Y%m%d%H%M%S'`
	echo UNIQID=`hostid`
	echo NISDOMAIN=`domainname`
	echo UNAME=`uname -a`
	echo RUNBY=`id`
	echo ISRV_Type=2048
	do_vm_detection
	echo '</CONTROL>'
	echo ""
}

# **************************************************************************
#  Main Routine
# **************************************************************************
PATH=/bin:/usr/bin:/usr/sbin:/sbin:/usr/contrib/bin
export PATH

LANG=C
export LANG
umask 022

# Directory to hold temporary files
MYTMPDIR=${MRMPDATADIR:-/tmp}
if [ ! -d "$MYTMPDIR" ] ; then
  exit
fi

# Initial setup
CONFIGFILE=$MRMPDIR/tmp/mrmpconfigdsc.sh
if [ ! -r "$CONFIGFILE" ] ; then
  CONFIGFILE=./mrmpconfigdsc.sh
fi
if [ -r "$CONFIGFILE" ]; then
  . $CONFIGFILE
fi
do_control
if [ "$OS_TYPE" = "AIX" ] && [ "$SetARG_MAX" -eq 1 ] ; then
  ncargs=`chdev -l sys0 -a ncargs=128`
fi
exit 0