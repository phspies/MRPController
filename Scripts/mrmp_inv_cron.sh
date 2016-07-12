#!/bin/bash
#  (c) 2016 Dimension Data, All rights reserved.
# **************************************************************************
#  Module:  UNIX MRMP Inventory cron Script
#  Desc:    This is a script to be used for cron scheduling
# **************************************************************************

PATH=/bin:/usr/bin:/usr/sbin:/sbin:/usr/contrib/bin
export PATH

LANG=C
export LANG

# Top-level dir where everything is installed
MRMPBINDIR=`dirname $0`
if [ "$MRMPBINDIR" = "." ]; then
	MRMPBINDIR=`pwd`
fi
export MRMPDIR
MRMPDIR=`dirname $MRMPBINDIR`
export MRMPDIR

umask 022

# If the top-level dir does not exist, then silently exit
[ ! -d $MRMPDIR ] && exit
cd $MRMPDIR

DATE=`date '+%Y%m%d%H%M%S'`
HOST=`hostname`
OS_TYPE=`uname -s`
MACH_TYPE=`uname -m`

MRMPDATADIR=$MRMPDIR/output
export MRMPDATADIR

# Create the dir to hold the output data
[ ! -d $MRMPDATADIR ] && mkdir $MRMPDATADIR

TMPOUT=$MRMPDATADIR/TmpInv_${HOST}_${DATE}.tmp
OUTPUT=$MRMPDATADIR/Inv_${HOST}_${DATE}.txt

if [ "$OS_TYPE" = "SunOS" ] ; then
  ksh $MRMPDIR/bin/mrmp_inv.sh > $TMPOUT 2>/dev/null
else
  $MRMPDIR/bin/mrmp_inv.sh > $TMPOUT 2>/dev/null
fi

mv $TMPOUT $OUTPUT

exit 0
