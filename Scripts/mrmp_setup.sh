#  (c) 2016 Dimension Data, All rights reserved.
# **************************************************************************
#  Module:  MRMP UNIX Setup Script
#  Desc:    This is a script creates directories
# **************************************************************************

PATH=/bin:/usr/bin:/usr/sbin:/sbin:/usr/contrib/bin
export PATH

LANG=C
export LANG

# Top-level dir where everything is installed
MRMPDIR=`pwd`
[ -n "$1" ] && MRMPDIR=$1
MRMPBINDIR=$MRMPDIR/bin
MRMPDATADIR=$MRMPDIR/output

umask 022

# Create the directories if they do not exist
[ ! -d $MRMPDIR ] && mkdir -p $MRMPDIR
[ ! -d $MRMPBINDIR ] && mkdir -p $MRMPBINDIR
[ ! -d $MRMPDATADIR ] && mkdir -p $MRMPDATADIR

# Copy script files to proper location
cp ./mrmp_inv.sh $MRMPBINDIR/mrmp_inv.sh
cp ./mrmp_inv_cron.sh $MRMPBINDIR/mrmp_inv_cron.sh
cp ./mrmp_perf.sh $MRMPBINDIR/mrmp_perf.sh
cp ./mrmp_perf_cron.sh $MRMPBINDIR/mrmp_perf_cron.sh

# Set scripts to executable
[ ! -x $MRMPBINDIR/mrmp_inv.sh ] && chmod 755 $MRMPBINDIR/mrmp_inv.sh
[ ! -x $MRMPBINDIR/mrmp_inv_cron.sh ] && chmod 755 $MRMPBINDIR/mrmp_inv_cron.sh
[ ! -x $MRMPBINDIR/mrmp_perf.sh ] && chmod 755 $MRMPBINDIR/mrmp_perf.sh
[ ! -x $MRMPBINDIR/mrmp_perf_cron.sh ] && chmod 755 $MRMPBINDIR/mrmp_perf_cron.sh

# Create Config File(s)
if [ ! -r $MRMPDIR/mrmpconfigperf.sh ] ; then
  echo PerfDuration=4 > $MRMPDIR/mrmpconfigperf.sh
  echo PerfNumSamples=3 >> $MRMPDIR/mrmpconfigperf.sh
fi

# Install scripts into cron
crontab -l | grep -v '/mrmp_' > ./crontab.$$
echo '30 23 * * * '$MRMPDIR/bin/mrmp_inv_cron.sh >> ./crontab.$$
echo '0 * * * * '$MRMPDIR/bin/mrmp_perf_cron.sh >> ./crontab.$$
crontab ./crontab.$$ 2>/dev/null
rm -f ./crontab.$$
echo "MRMP UNIX/Linux setup is complete."
echo "Use crontab -l to verify installation."

exit 0
