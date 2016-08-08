#!/bin/bash
#  (c) 2016 Dimension Data, All rights reserved.
# **************************************************************************
#  @(#)Module:  UNIX MRMP Performance Script
#  @(#)Desc:    This script runs all the UNIX/Linux utilities to collect performance
# **************************************************************************

# **************************************************************************
#  Control Section
# **************************************************************************
log()
{
  OS_TYPE=`uname -s`
  CPU_TYPE=`uname -m`
  echo '<TOOLS>'
  echo HOSTNAME=`hostname`
/  echo FILETYPE=Log
  echo DATE=`date -uR`
  echo TIMESTAMP=`date '+%Y%m%d%H%M%S'`
  echo CMD=$COMMAND
  echo ERR=$VAL
  echo '</TOOLS>'
  echo ""''
}

test_tools()
{
  check=0
  TMP=`which $cmd`
  COMMAND=`$cmd 1> tmpout_JGKGN_OJLHJ_DFJLM_HKGWP 2> tmperr_LHGIG_JBFJO_LJBFJ_NVGIO`
  if [ -x "$TMP" ] && ([ -s tmpout_JGKGN_OJLHJ_DFJLM_HKGWP ] || [ -s tmperr_LHGIG_JBFJO_LJBFJ_NVGIO ]) ; then
      check=1
  elif [ -n "$LOG_ERR" ] ; then
    COMMAND="$cmd"
    if [ -x "$TMP" ] ; then
      VAL="Command did not output any data"
    else
      if [ -s tmperr_LHGIG_JBFJO_LJBFJ_NVGIO ] ; then
	  VAL=`cat tmperr_LHGIG_JBFJO_LJBFJ_NVGIO`
      else
	  VAL="not found in $PATH"
      fi
    fi
    log
  fi
  `rm -rf tmperr_LHGIG_JBFJO_LJBFJ_NVGIO`
  `rm -rf tmpout_JGKGN_OJLHJ_DFJLM_HKGWP`
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
	OS_TYPE=`uname -s`
	CPU_TYPE=`uname -m`

	echo '<CONTROL>'
	echo VERSION=100
	echo FILETYPE=Perf
	echo DATE=`date -uR`
	echo TIMESTAMP=`date '+%Y%m%d%H%M%S'`
	if [ "$OS_TYPE" = "HP-UX" ] ; then
	  echo UNIQID=`uname -i`
        echo HOSTNAME=`hostname`
	else
	  echo UNIQID=`hostid`
        echo HOSTNAME=`uname -n`
	fi
	echo NISDOMAIN=`domainname 2> /dev/null`
	echo UNAME=`uname -a`
	echo RUNBY=`id`
	echo ISRV_Type=2048
	do_vm_detection
	echo '</CONTROL>'
	echo ""

	LOG_ERR=1
}

writeperf() {
  #PERFCLASS,PERFINSTANCE,PERFMETRIC,PERFTIME,PERFINT,PERFSAMP,PERFAVG,PERFMIN,PERFMAX
  PERFTIME=`date '+%Y%m%d%H%M%S'`

  echo '<PERF>'
  echo PERS_Type=4
  echo PERS_Active=1
  echo PERS_ClassName=$PERFCLASS
  echo PERS_InstanceName=$PERFINSTANCE
  echo PERS_MetricName=$PERFMETRIC
  echo PERS_LCID=1033
  echo PERD_CounterTime=$PERFTIME
  echo PERD_CounterInterval=$PERFINT
  echo PERD_CounterSamples=$PERFSAMP
  echo PERD_CounterAvg=$PERFAVG
  echo PERD_CounterMin=$PERFMIN
  echo PERD_CounterMax=$PERFMAX
  echo '</PERF>'
  echo ''
}

test_general_tools()
{
  cmd="awk"
  test_tools
  [ "$check" -eq 0 ] && exit 0
}

hpux_vmstat()
{
  cmd="vmstat"
  test_tools
  [ "$check" -eq 0 ] && return

  VMINTERVAL=$PerfDuration
  let VMSAMPLES=PerfNumSamples+1
  PAGESIZE=`getconf PAGESIZE`

  vmstat $VMINTERVAL $VMSAMPLES |
  ( VMSAMPLES=$PerfNumSamples
    read TITLE1
    read TITLE2

    # Read the first set of data
    read IGNORED
    # Exclude first set of data as it is an average since last reboot
    read min_0 min_1 min_2 min_3 min_4 min_5 min_6 min_7 min_8 min_9 min_10 min_11 min_12 min_13 min_14 min_15 min_16 min_17 IGNORED

    # Set max and totals
    i=0
    while [ $i -lt 18 ] ; do
      eval max_$i=\$min_$i
      eval tot_$i=\$min_$i
      i=`expr $i + 1`
    done
    min_pg=`expr $min_7 + $min_8`
    max_pg=$min_pg
    tot_pg=$min_pg

    # Now process the rest of the data
    while read f_0 f_1 f_2 f_3 f_4 f_5 f_6 f_7 f_8 f_9 f_10 f_11 f_12 f_13 f_14 f_15 f_16 f_17 IGNORED ; do
      i=0
      while [ $i -lt 18 ] ; do
        # Update totals
        eval tot_$i=`eval expr \\$tot_$i + \\$f_$i`

        # Update min value
        eval tmpf=\$f_$i
        eval tmpmin=\$min_$i
        if [ "$tmpf" -lt "$tmpmin" ] ; then
          eval min_$i=\$f_$i
        fi

        # Update max value
        eval tmpmax=\$max_$i
        if [ "$tmpf" -gt "$tmpmax" ] ; then
          eval max_$i=\$f_$i
        fi
        let i=i+1
      done

      tmpf=`expr $f_7 + $f_8`
      if [ "$tmpf" -lt "$min_pg" ] ; then
        min_pg=$tmpf
      fi
      if [ "$tmpf" -gt "$tmpmax" ] ; then
        max_pg=$tmpf
      fi
      tot_pg=`expr $tot_7 + $tot_8`
    done

    # Now print out the metrics

    # Processes in various states
    PERFCLASS=System;PERFINSTANCE='_Total';PERFMETRIC='Processor Queue Length';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`awk -v a=$tot_0 -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;PERFMIN=$min_0;PERFMAX=$max_0;writeperf
    PERFCLASS=System;PERFINSTANCE='_Total';PERFMETRIC='Blocked Processes';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`awk -v a=$tot_1 -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;PERFMIN=$min_1;PERFMAX=$max_1;writeperf
    PERFCLASS=System;PERFINSTANCE='_Total';PERFMETRIC='Swapped Runnable Processes';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`awk -v a=$tot_2 -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;PERFMIN=$min_2;PERFMAX=$max_2;writeperf

    # Memory
    PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Active Virtual Pages';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`awk -v a=$tot_3 -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;PERFMIN=$min_3;PERFMAX=$max_3;writeperf
    PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Free List';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`awk -v a=$tot_4 -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;PERFMIN=$min_4;PERFMAX=$max_4;writeperf
    PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Available Bytes';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`echo \( $tot_4 \* $PAGESIZE \) / $VMSAMPLES | bc -l`;PERFMIN=`echo $min_4 \* $PAGESIZE | bc -l`;PERFMAX=`echo $max_4 \* $PAGESIZE | bc -l`;writeperf

    # Paging
    PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Pages Reclaimed/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`awk -v a=$tot_5 -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;PERFMIN=$min_5;PERFMAX=$max_5;writeperf
    PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Address Translation Faults/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`awk -v a=$tot_6 -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;PERFMIN=$min_6;PERFMAX=$max_6;writeperf
    PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Pages Input/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`awk -v a=$tot_7 -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;PERFMIN=$min_7;PERFMAX=$max_7;writeperf
    PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Pages Output/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`awk -v a=$tot_8 -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;PERFMIN=$min_8;PERFMAX=$max_8;writeperf
    PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Pages/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`echo $tot_pg / $VMSAMPLES | bc -l`;PERFMIN=$min_pg;PERFMAX=$max_pg;writeperf
    PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Pages Freed/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`awk -v a=$tot_9 -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;PERFMIN=$min_9;PERFMAX=$max_9;writeperf
    PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Anticipated Shortfall';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`awk -v a=$tot_10 -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;PERFMIN=$min_10;PERFMAX=$max_10;writeperf
    PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Pages Scanned/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`awk -v a=$tot_11 -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;PERFMIN=$min_11;PERFMAX=$max_11;writeperf

    # Faults
    PERFCLASS=Processor;PERFINSTANCE='_Total';PERFMETRIC='Interrupts/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`awk -v a=$tot_12 -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;PERFMIN=$min_12;PERFMAX=$max_12;writeperf
    PERFCLASS=System;PERFINSTANCE='_Total';PERFMETRIC='System Calls/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`awk -v a=$tot_13 -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;PERFMIN=$min_13;PERFMAX=$max_13;writeperf
    PERFCLASS=System;PERFINSTANCE='_Total';PERFMETRIC='Context Switches/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`awk -v a=$tot_14 -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;PERFMIN=$min_14;PERFMAX=$max_14;writeperf

    # CPU
    PERFCLASS=Processor;PERFINSTANCE='_Total';PERFMETRIC='% User Time';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`awk -v a=$tot_15 -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;PERFMIN=$min_15;PERFMAX=$max_15;writeperf
    PERFCLASS=Processor;PERFINSTANCE='_Total';PERFMETRIC='% System Time';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`awk -v a=$tot_16 -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;PERFMIN=$min_16;PERFMAX=$max_16;writeperf
    PERFCLASS=Processor;PERFINSTANCE='_Total';PERFMETRIC='% Idle Time';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`awk -v a=$tot_17 -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;PERFMIN=$min_17;PERFMAX=$max_17;writeperf
    PERFCLASS=Processor;PERFINSTANCE='_Total';PERFMETRIC='% Processor Time';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`awk -v a=$tot_17 -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", 100-a/b) }'`;PERFMIN=`awk -v a=$max_17 'BEGIN { printf("%.5f\n", 100-a) }'`;PERFMAX=`awk -v a=$min_17 'BEGIN { printf("%.5f\n", 100-a) }'`;writeperf
  )
}

linux_vmstat()
{
  cmd="vmstat"
  test_tools
  [ "$check" -eq 0 ] && return

	VMINTERVAL=$PerfDuration
	let VMSAMPLES=PerfNumSamples+1

	vmstat -n $VMINTERVAL $VMSAMPLES |
	( 
		VMSAMPLES=$PerfNumSamples
		read TITLE1
		read TITLE2

		# Get index of the counters from the Header. If a counter is missing the index is left un-initialised.
		# The index is checked for initialization before being used in the processing that follows.
		hdr_r=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "r") print i - 1;}'`
		hdr_b=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "b") print i - 1;}'`
		hdr_w=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "w") print i - 1;}'`
		hdr_swpd=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "swpd") print i - 1;}'`
		hdr_free=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "free") print i - 1;}'`
		hdr_buff=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "buff") print i - 1;}'`
		hdr_cache=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "cache") print i - 1;}'`
		# hdr_inact=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "inact") print i - 1;}'`
		# hdr_active=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "active") print i - 1;}'`
		# hdr_si=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "si") print i - 1;}'`
		# hdr_so=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "so") print i - 1;}'`
		hdr_bi=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "bi") print i - 1;}'`
		hdr_bo=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "bo") print i - 1;}'`
		hdr_in=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "in") print i - 1;}'`
		hdr_cs=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "cs") print i - 1;}'`
		hdr_us=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "us") print i - 1;}'`
		hdr_sy=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "sy") print i - 1;}'`
		hdr_id=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "id") print i - 1;}'`
		# hdr_wa=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "wa") print i - 1;}'`

		# Read the first set of data
		read IGNORED
		# Exclude first set of data as it is an average since last reboot
		read min_0 min_1 min_2 min_3 min_4 min_5 min_6 min_7 min_8 min_9 min_10 min_11 min_12 min_13 min_14 min_15 IGNORED

		# Set max and totals
		i=0
		while [ $i -lt 16 ] ; do
			eval max_$i=\$min_$i
			eval tot_$i=\$min_$i
			let i=i+1
		done
		if [ -n "$hdr_bi" ] && [ -n "$hdr_bo" ] ; then
			tot_bt=`eval expr \\$tot_$hdr_bi + \\$tot_$hdr_bo`
			min_bt=$tot_bt
			max_bt=$tot_bt
		fi

		# Now process the rest of the data
		while read f_0 f_1 f_2 f_3 f_4 f_5 f_6 f_7 f_8 f_9 f_10 f_11 f_12 f_13 f_14 f_15 IGNORED ; do
			i=0
			while [ $i -lt 16 ] ; do
				# Update totals
				eval tot_$i=`eval expr \\$tot_$i + \\$f_$i`

				# Update min value
				eval tmpf=\$f_$i
				eval tmpmin=\$min_$i
				if [ "$tmpf" -lt "$tmpmin" ] ; then
					eval min_$i=\$f_$i
				fi

				# Update max value
				eval tmpmax=\$max_$i
				if [ "$tmpf" -gt "$tmpmax" ] ; then
					eval max_$i=\$f_$i
				fi
				let i=i+1
			done

			if [ -n "$hdr_bi" ] && [ -n "$hdr_bo" ] ; then
				tmpf=`eval expr \\$f_$hdr_bi + \\$f_$hdr_bo`
				let tot_bt=tot_bt+tmpf
				if [ "$tmpf" -lt "$min_bt" ] ; then
					min_bt=$tmpf
				fi
				if [ "$tmpf" -gt "$max_bt" ] ; then
					max_bt=$tmpf
				fi
			fi
		done

		# Now print out the metrics

		# Processes in various states
		if [ -n "$hdr_r" ] ; then
			PERFCLASS=System;PERFINSTANCE='_Total';PERFMETRIC='Processor Queue Length';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
			eval PERFAVG=\$tot_$hdr_r
			PERFAVG=`awk -v a=$PERFAVG -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;eval PERFMIN=\$min_$hdr_r;eval PERFMAX=\$max_$hdr_r
			writeperf
		fi
		if [ -n "$hdr_b" ] ; then
			PERFCLASS=System;PERFINSTANCE='_Total';PERFMETRIC='Blocked Processes';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
			eval PERFAVG=\$tot_$hdr_b
			PERFAVG=`awk -v a=$PERFAVG -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;eval PERFMIN=\$min_$hdr_b;eval PERFMAX=\$max_$hdr_b
			writeperf
		fi
		if [ -n "$hdr_w" ] ; then
			PERFCLASS=System;PERFINSTANCE='_Total';PERFMETRIC='Swapped Runnable Processes';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
			eval PERFAVG=\$tot_$hdr_w
			PERFAVG=`awk -v a=$PERFAVG -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;eval PERFMIN=\$min_$hdr_w;eval PERFMAX=\$max_$hdr_w
			writeperf
		fi

		# Memory
		if [ -n "$hdr_swpd" ] ; then
			PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Virtual Memory Used (KB)';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
			eval PERFAVG=\$tot_$hdr_swpd
			PERFAVG=`awk -v a=$PERFAVG -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;eval PERFMIN=\$min_$hdr_swpd;eval PERFMAX=\$max_$hdr_swpd
			writeperf
		fi
		if [ -n "$hdr_free" ] ; then
			PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Free List';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
			eval PERFAVG=\$tot_$hdr_free
			PERFAVG=`awk -v a=$PERFAVG -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;eval PERFMIN=\$min_$hdr_free;eval PERFMAX=\$max_$hdr_free
			writeperf

			# Available Bytes is Free List * 1024
			PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Available Bytes';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
			PERFAVG=`awk -v a=$PERFAVG 'BEGIN { printf("%.5f\n", a*1024) }'`
			PERFMIN=`awk -v a=$PERFMIN 'BEGIN { printf("%.5f\n", a*1024) }'`
			PERFMAX=`awk -v a=$PERFMAX 'BEGIN { printf("%.5f\n", a*1024) }'`
			writeperf
		fi

		if [ -n "$hdr_buff" ] ; then
			PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Used Buffer Memory (KB)';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
			eval PERFAVG=\$tot_$hdr_buff
			PERFAVG=`awk -v a=$PERFAVG -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;eval PERFMIN=\$min_$hdr_buff;eval PERFMAX=\$max_$hdr_buff
			writeperf
		fi

		if [ -z "$hdr_cache" ] && [ -r /proc/meminfo ] ; then
			PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Cache Bytes';PERFINT=0;PERFSAMP=1
			PERFAVG=`awk -v a=$VMSAMPLES '/^Mem:/ { print $7 }' /proc/meminfo`
			PERFMIN=$PERFAVG
			PERFMAX=$PERFAVG
			writeperf
		fi
		if [ -n "$hdr_cache" ] ; then
			PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Cache Bytes';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
			eval PERFAVG=\$tot_$hdr_cache
			eval PERFMIN=\$min_$hdr_cache
			eval PERFMAX=\$max_$hdr_cache
			PERFAVG=`awk -v a=$PERFAVG -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a*1024/b) }'`;PERFMIN=`awk -v a=$PERFMIN 'BEGIN { printf("%.5f\n", a*1024) }'`;PERFMAX=`awk -v a=$PERFMAX 'BEGIN { printf("%.5f\n", a*1024) }'`
			writeperf
		fi

		if [ -n "$hdr_bi" ] ; then
			PERFCLASS=PhysicalDisk;PERFINSTANCE="_Total";PERFMETRIC='Disk Blocks In/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
			eval blocks_in=\$tot_$hdr_bi;
			PERFAVG=`awk -v a=$blocks_in -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`
			eval PERFMIN=\$min_$hdr_bi
			eval PERFMAX=\$max_$hdr_bi
			writeperf

			PERFCLASS=PhysicalDisk;PERFINSTANCE="_Total";PERFMETRIC='Disk Bytes In/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
			PERFAVG=`awk -v a=$blocks_in -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a*1024/b) }'`
			let PERFMIN=PERFMIN*1024
			let PERFMAX=PERFMAX*1024
			writeperf
		fi

		if [ -n "$hdr_bo" ] ; then
			PERFCLASS=PhysicalDisk;PERFINSTANCE="_Total";PERFMETRIC='Disk Blocks Out/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
			eval blocks_out=\$tot_$hdr_bo;
			PERFAVG=`awk -v a=$blocks_out -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`
			eval PERFMIN=\$min_$hdr_bo
			eval PERFMAX=\$max_$hdr_bo
			writeperf

			PERFCLASS=PhysicalDisk;PERFINSTANCE="_Total";PERFMETRIC='Disk Bytes Out/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
			PERFAVG=`awk -v a=$blocks_out -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a*1024/b) }'`
			let PERFMIN=PERFMIN*1024
			let PERFMAX=PERFMAX*1024
			writeperf
		fi

		if [ -n "$hdr_bi" ] && [ -n "$hdr_bo" ] ; then
			PERFCLASS=PhysicalDisk;PERFINSTANCE="_Total";PERFMETRIC='Disk Blocks/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
			blocks_tot=$tot_bt;
			PERFAVG=`awk -v a=$blocks_tot -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`
			PERFMIN=$min_bt
			PERFMAX=$max_bt
			writeperf

			PERFCLASS=PhysicalDisk;PERFINSTANCE="_Total";PERFMETRIC='Disk Bytes/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
			PERFAVG=`awk -v a=$blocks_tot -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a*1024/b) }'`
			let PERFMIN=PERFMIN*1024
			let PERFMAX=PERFMAX*1024
			writeperf
		fi

		tot_time=`awk '{print $1}' /proc/uptime`
		page_line=`grep 'page' /proc/stat`
		if [ -n "$page_line" ] ; then
			pages_in=`echo $page_line | awk '{print $2}'`
			pages_out=`echo $page_line | awk '{print $3}'`
		else
			pages_in=`grep 'pgpgin' /proc/vmstat | awk '{print $2}'`
			pages_out=`grep 'pgpgout' /proc/vmstat | awk '{print $2}'`
		fi
		pages_avg=`awk -v a=$pages_in -v b=$pages_out -v c=$tot_time 'BEGIN { print (a+b)/c }'`
		PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Pages/sec';PERFINT=0;PERFSAMP=1
		PERFAVG=`echo $pages_avg`;eval PERFMIN=$pages_avg;eval PERFMAX=$pages_avg
		writeperf

		# Faults
		if [ -n "$hdr_in" ] ; then
			PERFCLASS=Processor;PERFINSTANCE='_Total';PERFMETRIC='Interrupts/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
			eval PERFAVG=\$tot_$hdr_in
			PERFAVG=`awk -v a=$PERFAVG -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`
			eval PERFMIN=\$min_$hdr_in
			eval PERFMAX=\$max_$hdr_in
			writeperf
		fi
		if [ -n "$hdr_cs" ] ; then
			PERFCLASS=System;PERFINSTANCE='_Total';PERFMETRIC='Context Switches/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
			eval PERFAVG=\$tot_$hdr_cs
			PERFAVG=`awk -v a=$PERFAVG -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`
			eval PERFMIN=\$min_$hdr_cs
			eval PERFMAX=\$max_$hdr_cs
			writeperf
		fi

		# CPU
		if [ -n "$hdr_us" ] ; then
			PERFCLASS=Processor;PERFINSTANCE='_Total';PERFMETRIC='% User Time';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
			eval PERFAVG=\$tot_$hdr_us
			PERFAVG=`awk -v a=$PERFAVG -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`
			eval PERFMIN=\$min_$hdr_us
			eval PERFMAX=\$max_$hdr_us
			writeperf
		fi
		if [ -n "$hdr_sy" ] ; then
			PERFCLASS=Processor;PERFINSTANCE='_Total';PERFMETRIC='% System Time';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
			eval PERFAVG=\$tot_$hdr_sy
			PERFAVG=`awk -v a=$PERFAVG -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`
			eval PERFMIN=\$min_$hdr_sy
			eval PERFMAX=\$max_$hdr_sy
			writeperf
		fi
		if [ -n "$hdr_id" ] ; then
			PERFCLASS=Processor;PERFINSTANCE='_Total';PERFMETRIC='% Idle Time';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
			eval PERFAVG=\$tot_$hdr_id
			PERFAVG=`awk -v a=$PERFAVG -v b=$VMSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`
			eval PERFMIN=\$min_$hdr_id
			eval PERFMAX=\$max_$hdr_id
			writeperf
			PERFCLASS=Processor;PERFINSTANCE='_Total';PERFMETRIC='% Processor Time';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
			PERFAVG=`awk -v a=$PERFAVG 'BEGIN { printf("%.5f\n", 100-a) }'`
			PERFMIN=`awk -v a=$PERFMAX 'BEGIN { printf("%.5f\n", 100-a) }'`
			PERFMAX=`awk -v a=$PERFMIN 'BEGIN { printf("%.5f\n", 100-a) }'`
			writeperf
		fi
	)
}

aix_get_cache()
{
  cmd="cachefsstat"
  test_tools
  [ "$check" -eq 0 ] && return
}

# This is very similar to the HP case ... extra columns in the middle
sun_vmstat()
{
  cmd="vmstat"
  test_tools
  [ "$check" -eq 0 ] && return

  VMINTERVAL=$PerfDuration
  VMSAMPLES=`expr $PerfNumSamples + 1`
  PAGESIZE=`getconf PAGESIZE`
  PAGESIZE=`echo $PAGESIZE / 1024 | bc -l`

  vmstat $VMINTERVAL $VMSAMPLES |
  (
    VMSAMPLES=$PerfNumSamples
    read TITLE1
    read TITLE2

    # Read the first set of data
    read IGNORED
    # Exclude first set of data as it is an average since last reboot
    read min_0 min_1 min_2 min_3 min_4 min_5 min_6 min_7 min_8 min_9 min_10 min_11 min_12 min_13 min_14 min_15 min_16 min_17 min_18 min_19 min_20 min_21 IGNORED

    # Set max and totals
    i=0
    while [ $i -lt 22 ] ; do
      eval max_$i=\$min_$i
      eval tot_$i=\$min_$i
      i=`expr $i + 1`
    done
    min_pg=`expr $min_7 + $min_8`
    max_pg=$min_pg
    tot_pg=$min_pg

    # Now process the rest of the data
    while read f_0 f_1 f_2 f_3 f_4 f_5 f_6 f_7 f_8 f_9 f_10 f_11 f_12 f_13 f_14 f_15 f_16 f_17 f_18 f_19 f_20 f_21 IGNORED ; do
      i=0
      while [ $i -lt 22 ] ; do
        # Update totals
        eval tot_$i=`eval expr \\$tot_$i + \\$f_$i`

        # Update min value
        eval tmpf=\$f_$i
        eval tmpmin=\$min_$i
        if [ "$tmpf" -lt "$tmpmin" ] ; then
          eval min_$i=\$f_$i
        fi

        # Update max value
        eval tmpmax=\$max_$i
        if [ "$tmpf" -gt "$tmpmax" ] ; then
          eval max_$i=\$f_$i
        fi
        i=`expr $i + 1`
      done
      tmpf=`expr $f_7 + $f_8`
      if [ "$tmpf" -lt "$min_pg" ] ; then
        min_pg=$tmpf
      fi
      if [ "$tmpf" -gt "$max_pg" ] ; then
        max_pg=$tmpf
      fi
      tot_pg=`expr $tot_7 + $tot_8`
    done

    # Now print out the metrics

    # Processes in various states
    PERFCLASS=System;PERFINSTANCE='_Total';PERFMETRIC='Processor Queue Length';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`echo $tot_0 / $VMSAMPLES | bc -l`;PERFMIN=$min_0;PERFMAX=$max_0;writeperf
    PERFCLASS=System;PERFINSTANCE='_Total';PERFMETRIC='Blocked Processes';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`echo $tot_1 / $VMSAMPLES | bc -l`;PERFMIN=$min_1;PERFMAX=$max_1;writeperf
    PERFCLASS=System;PERFINSTANCE='_Total';PERFMETRIC='Swapped Runnable Processes';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`echo $tot_2 / $VMSAMPLES | bc -l`;PERFMIN=$min_2;PERFMAX=$max_2;writeperf

    # Memory
    PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Active Virtual Pages';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`echo $tot_3 / $VMSAMPLES | bc -l`;PERFMIN=$min_3;PERFMAX=$max_3;writeperf
    PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Free List';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`echo $tot_4 / $VMSAMPLES | bc -l`;PERFMIN=$min_4;PERFMAX=$max_4;writeperf
    PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Available Bytes';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`echo \( $tot_4 \* 1024 \) / $VMSAMPLES | bc -l`;PERFMIN=`echo $min_4 \* 1024 | bc -l`;PERFMAX=`echo $max_4 \* 1024 | bc -l`;writeperf

    # Paging
    PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Pages Reclaimed/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`echo $tot_5 / $VMSAMPLES | bc -l`;PERFMIN=$min_5;PERFMAX=$max_5;writeperf
    PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Address Translation Faults/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`echo $tot_6 / $VMSAMPLES | bc -l`;PERFMIN=$min_6;PERFMAX=$max_6;writeperf
    # We are interested in the Pages/sec counter only. The units of the following two counters are not in Pages but Kb.
    PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Pages Input/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`echo $tot_7 / \( $VMSAMPLES \* $PAGESIZE \) | bc -l`;PERFMIN=`echo $min_7 / $PAGESIZE | bc -l`;PERFMAX=`echo $max_7 / $PAGESIZE | bc -l`;writeperf
    PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Pages Output/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`echo $tot_8 / \( $VMSAMPLES \* $PAGESIZE \) | bc -l`;PERFMIN=`echo $min_8 / $PAGESIZE | bc -l`;PERFMAX=`echo $max_8 / $PAGESIZE | bc -l`;writeperf
    PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Pages/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`echo $tot_pg / \( $VMSAMPLES \* $PAGESIZE \) | bc -l`;PERFMIN=`echo $min_pg / $PAGESIZE | bc -l`;PERFMAX=`echo $max_pg / $PAGESIZE | bc -l`;writeperf

    PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Pages Freed/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`echo $tot_9 / $VMSAMPLES | bc -l`;PERFMIN=$min_9;PERFMAX=$max_9;writeperf
    #PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Anticipated Shortfall';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`echo $tot_10 / $VMSAMPLES | bc -l`;PERFMIN=$min_10;PERFMAX=$max_10;writeperf
    #PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Pages Scanned/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`echo $tot_11 / $VMSAMPLES | bc -l`;PERFMIN=$min_11;PERFMAX=$max_11;writeperf

    # Faults
    PERFCLASS=Processor;PERFINSTANCE='_Total';PERFMETRIC='Interrupts/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`echo $tot_16 / $VMSAMPLES | bc -l`;PERFMIN=$min_16;PERFMAX=$max_16;writeperf
    PERFCLASS=System;PERFINSTANCE='_Total';PERFMETRIC='System Calls/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`echo $tot_17 / $VMSAMPLES | bc -l`;PERFMIN=$min_17;PERFMAX=$max_17;writeperf
    PERFCLASS=System;PERFINSTANCE='_Total';PERFMETRIC='Context Switches/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`echo $tot_18 / $VMSAMPLES | bc -l`;PERFMIN=$min_18;PERFMAX=$max_18;writeperf

    # CPU
    PERFCLASS=Processor;PERFINSTANCE='_Total';PERFMETRIC='% User Time';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`echo $tot_19 / $VMSAMPLES | bc -l`;PERFMIN=$min_19;PERFMAX=$max_19;writeperf
    PERFCLASS=Processor;PERFINSTANCE='_Total';PERFMETRIC='% System Time';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`echo $tot_20 / $VMSAMPLES | bc -l`;PERFMIN=$min_20;PERFMAX=$max_20;writeperf
    PERFCLASS=Processor;PERFINSTANCE='_Total';PERFMETRIC='% Idle Time';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`echo $tot_21 / $VMSAMPLES | bc -l`;PERFMIN=$min_21;PERFMAX=$max_21;writeperf
    PERFCLASS=Processor;PERFINSTANCE='_Total';PERFMETRIC='% Processor Time';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES;PERFAVG=`echo 100 - \( $tot_21 / $VMSAMPLES \) | bc -l`;PERFMIN=`expr 100 - $max_21`;PERFMAX=`expr 100 - $min_21`;writeperf
  )
}

aix_vmstat()
{
  cmd="vmstat"
  test_tools
  [ "$check" -eq 0 ] && return

  VMINTERVAL=$PerfDuration
  VMSAMPLES=`expr $PerfNumSamples + 1`

  vmstat $VMINTERVAL $VMSAMPLES | awk '{if($1 == "kthr")flag=1;if(flag)print $0;}' |
  (
    VMSAMPLES=$PerfNumSamples
    read TITLE1
    read TITLEFMT
    read TITLE2

#   Get index of the counters from the Header. If a counter is missing the index is left un-initialised.
#   The index is checked for initialization before being used in the processing that follows.
    hdr_r=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "r") { print i - 1; break;} }'`
    hdr_b=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "b") { print i - 1; break;} }'`
#   hdr_p=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "p") { print i - 1; break;} }'`
    hdr_avm=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "avm") { print i - 1; break;} }'`
    hdr_fre=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "fre") { print i - 1; break;} }'`
#   hdr_re=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "re") { print i - 1; break;} }'`
    hdr_pi=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "pi") { print i - 1; break;} }'`
    hdr_po=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "po") { print i - 1; break;} }'`
#   hdr_fr=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "fr") { print i - 1; break;} }'`
#   hdr_sr=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "sr") { print i - 1; break;} }'`
#   hdr_cy=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "cy") { print i - 1; break;} }'`
#   hdr_fi=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "fi") { print i - 1; break;} }'`
#   hdr_fo=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "fo") { print i - 1; break;} }'`
    hdr_in=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "in") { print i - 1; break;} }'`
    hdr_fsy=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "sy") { print i - 1; break;} }'`
    hdr_cs=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "cs") { print i - 1; break;} }'`
    hdr_us=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "us") { print i - 1; break;} }'`
    if [ -n "$hdr_fsy" ] ; then 
      hdr_csy=`echo $TITLE2 | awk 'BEGIN {FS=" ";tmp=0;} {for(i=1;i<=NF;i++) if ($i == "sy" && tmp) { print i - 1; break; } else if ($i == "sy") {tmp++;} }'`
    fi
    hdr_id=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "id") { print i - 1; break;} }'`
#   hdr_wa=`echo $TITLE2 | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "wa") { print i - 1; break;} }'`

    # Read the first set of data
    read IGNORED
    # Exclude first set of data as it is an average since last reboot
    read min_0 min_1 min_2 min_3 min_4 min_5 min_6 min_7 min_8 min_9 min_10 min_11 min_12 min_13 min_14 min_15 min_16 IGNORED

    # Set max and totals
    i=0
    while [ $i -lt 17 ] ; do
      eval max_$i=\$min_$i
      eval tot_$i=\$min_$i
      i=`expr $i + 1`
    done
    min_pg=`eval expr \\$min_$hdr_pi + \\$min_$hdr_po`
    max_pg=$min_pg
    tot_pg=$min_pg

    # Now process the rest of the data
    while read f_0 f_1 f_2 f_3 f_4 f_5 f_6 f_7 f_8 f_9 f_10 f_11 f_12 f_13 f_14 f_15 f_16 IGNORED ; do
      i=0
      while [ $i -lt 17 ] ; do
        # Update totals
        eval tot_$i=`eval expr \\$tot_$i + \\$f_$i`

        # Update min value
        eval tmpf=\$f_$i
        eval tmpmin=\$min_$i
        if [ "$tmpf" -lt "$tmpmin" ] ; then
          eval min_$i=\$f_$i
        fi

        # Update max value
        eval tmpmax=\$max_$i
        if [ "$tmpf" -gt "$tmpmax" ] ; then
          eval max_$i=\$f_$i
        fi

        i=`expr $i + 1`
      done
      if [ \( -n "$hdr_po" \) -o \( -n "$hdr_pi" \) ] ; then 
        tmpf=`eval expr \\$f_$hdr_pi + \\$f_$hdr_po`
        if [ "$tmpf" -lt "$min_pg" ] ; then
          min_pg=$tmpf
        fi
        if [ "$tmpf" -gt "$max_pg" ] ; then
          max_pg=$tmpf
        fi
        tot_pg=`eval expr \\$tot_$hdr_pi + \\$tot_$hdr_po`
      fi
    done

    # Now print out the metrics

    # Processes in various states
    if [ -n "$hdr_r" ] ; then 
      PERFCLASS=System;PERFINSTANCE='_Total';PERFMETRIC='Processor Queue Length';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
      PERFAVG=`eval echo \\$tot_$hdr_r / $VMSAMPLES | bc -l`;eval PERFMIN=\$min_$hdr_r;eval PERFMAX=\$max_$hdr_r
      writeperf
    fi
    if [ -n "$hdr_b" ] ; then 
      PERFCLASS=System;PERFINSTANCE='_Total';PERFMETRIC='Blocked Processes';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
      PERFAVG=`eval echo \\$tot_$hdr_b / $VMSAMPLES | bc -l`;eval PERFMIN=\$min_$hdr_b;eval PERFMAX=\$max_$hdr_b
      writeperf
    fi

    # Memory
    cmd="pagesize"
    test_tools
    if [ "$check" -eq 1 ] ; then
      PAGESIZE=`pagesize`
      if [ -n "$hdr_avm" ] ; then
        PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Virtual Memory Used (KB)';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
        PERFAVG=`eval echo "\( \\$tot_$hdr_avm \* $PAGESIZE \) / \( $VMSAMPLES \* 1024 \)" | bc -l`
        PERFMIN=`eval echo "\( \\$min_$hdr_avm \* $PAGESIZE \) / 1024" | bc -l`
        PERFMAX=`eval echo "\( \\$max_$hdr_avm \* $PAGESIZE \) / 1024" | bc -l`
        writeperf
      fi
    fi
    if [ -n "$hdr_fre" ] ; then 
      PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Free List';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
      PERFAVG=`eval echo \\$tot_$hdr_fre / $VMSAMPLES | bc -l`;eval PERFMIN=\$min_$hdr_fre;eval PERFMAX=\$max_$hdr_fre
      writeperf

      if [ "$check" -eq 1 ] ; then
        PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Available Bytes';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
        PERFAVG=`echo "$PERFAVG * $PAGESIZE" | bc -l`;PERFMIN=`echo "$PERFMIN * $PAGESIZE" | bc -l`;PERFMAX=`echo "$PERFMAX * $PAGESIZE" | bc -l`
        writeperf
      fi
    fi

    # Paging
    if [ -n "$hdr_pi" ] ; then 
      PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Pages Input/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
      PERFAVG=`eval echo \\$tot_$hdr_pi / $VMSAMPLES | bc -l`;eval PERFMIN=\$min_$hdr_pi;eval PERFMAX=\$max_$hdr_pi
      writeperf
    fi
    if [ -n "$hdr_po" ] ; then 
      PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Pages Output/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
      PERFAVG=`eval echo \\$tot_$hdr_po / $VMSAMPLES | bc -l`;eval PERFMIN=\$min_$hdr_po;eval PERFMAX=\$max_$hdr_po
      writeperf
    fi
    if [ -n "$tot_pg" ] ; then 
      PERFCLASS=Memory;PERFINSTANCE='_Total';PERFMETRIC='Pages/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
      PERFAVG=`echo $tot_pg / $VMSAMPLES | bc -l`;PERFMIN=$min_pg;PERFMAX=$max_pg
      writeperf
    fi

    # Faults
    if [ -n "$hdr_in" ] ; then 
      PERFCLASS=Processor;PERFINSTANCE='_Total';PERFMETRIC='Interrupts/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
      PERFAVG=`eval echo \\$tot_$hdr_in / $VMSAMPLES | bc -l`;eval PERFMIN=\$min_$hdr_in;eval PERFMAX=\$max_$hdr_in
      writeperf
    fi
    if [ -n "$hdr_fsy" ] ; then 
      PERFCLASS=System;PERFINSTANCE='_Total';PERFMETRIC='System Calls/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
      PERFAVG=`eval echo \\$tot_$hdr_fsy / $VMSAMPLES | bc -l`;eval PERFMIN=\$min_$hdr_fsy;eval PERFMAX=\$max_$hdr_fsy
      writeperf
    fi
    if [ -n "$hdr_cs" ] ; then 
      PERFCLASS=System;PERFINSTANCE='_Total';PERFMETRIC='Context Switches/sec';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
      PERFAVG=`eval echo \\$tot_$hdr_cs / $VMSAMPLES | bc -l`;eval PERFMIN=\$min_$hdr_cs;eval PERFMAX=\$max_$hdr_cs
      writeperf
    fi

    # CPU
    if [ -n "$hdr_us" ] ; then 
      PERFCLASS=Processor;PERFINSTANCE='_Total';PERFMETRIC='% User Time';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
      PERFAVG=`eval echo \\$tot_$hdr_us / $VMSAMPLES | bc -l`;eval PERFMIN=\$min_$hdr_us;eval PERFMAX=\$max_$hdr_us
      writeperf
    fi
    if [ -n "$hdr_csy" ] ; then 
      PERFCLASS=Processor;PERFINSTANCE='_Total';PERFMETRIC='% System Time';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
      PERFAVG=`eval echo \\$tot_$hdr_csy / $VMSAMPLES | bc -l`;eval PERFMIN=\$min_$hdr_csy;eval PERFMAX=\$max_$hdr_csy
      writeperf
    fi
    if [ -n "$hdr_id" ] ; then 
      PERFCLASS=Processor;PERFINSTANCE='_Total';PERFMETRIC='% Idle Time';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
      PERFAVG=`eval echo \\$tot_$hdr_id / $VMSAMPLES | bc -l`;eval PERFMIN=\$min_$hdr_id;eval PERFMAX=\$max_$hdr_id
      writeperf
      PERFCLASS=Processor;PERFINSTANCE='_Total';PERFMETRIC='% Processor Time';PERFINT=$VMINTERVAL;PERFSAMP=$VMSAMPLES
      PERFAVG=`eval echo "100 - \( \\$tot_$hdr_id / $VMSAMPLES \) " | bc -l`;eval PERFMIN=`eval expr 100 - \\$max_$hdr_id`;eval PERFMAX=`eval expr 100 - \\$min_$hdr_id`
      writeperf
    fi
  )
}

do_fs()
{
  CMD="bdf -l"
  cmd="bdf"

  if [ "$OS_TYPE" = "Linux" ] ; then
    CMD="df -lP"
    cmd="df"
  fi

  if [ "$OS_TYPE" = "SunOS" ] ; then
    CMD="df -k -l"
    MNTTAB=/etc/mnttab
    cmd="df"
  fi

  if [ "$OS_TYPE" = "AIX" ] ; then
    CMD="df -kP"
    cmd="df"
  fi

  test_tools
  [ "$check" -eq 0 ] && return

  OUTPUT=`$CMD`

  if [ "$OS_TYPE" = "AIX" ] ; then
    OUTPUT=`echo "$OUTPUT"  | grep -v '^/proc'`
  fi

  echo "$OUTPUT" |
  ( # Skip the header-line
    read IGNORED

    while read FS SIZE USED AVAIL PCT MOUNTED ; do
      PCT=`echo $PCT | tr -d %`

      PERFCLASS=LogicalDisk;PERFINSTANCE=$FS;PERFMETRIC=Size;PERFINT=0;PERFSAMP=1;PERFAVG=$SIZE;PERFMIN=$SIZE;PERFMAX=$SIZE;writeperf
      PERFCLASS=LogicalDisk;PERFINSTANCE=$FS;PERFMETRIC=Used;PERFINT=0;PERFSAMP=1;PERFAVG=$USED;PERFMIN=$USED;PERFMAX=$USED;writeperf
      PERFCLASS=LogicalDisk;PERFINSTANCE=$FS;PERFMETRIC=Avail;PERFINT=0;PERFSAMP=1;PERFAVG=$AVAIL;PERFMIN=$AVAIL;PERFMAX=$AVAIL;writeperf
      PERFCLASS=LogicalDisk;PERFINSTANCE=$FS;PERFMETRIC='Free Megabytes';PERFINT=0;PERFSAMP=1;PERFAVG=`echo "$AVAIL 1024" | awk '{print $1/$2}'`;PERFMIN=$PERFAVG;PERFMAX=$PERFAVG;writeperf
      PERFCLASS=LogicalDisk;PERFINSTANCE=$FS;PERFMETRIC='% Space Used';PERFINT=0;PERFSAMP=1;PERFAVG=$PCT;PERFMIN=$PCT;PERFMAX=$PCT;writeperf
    done
  )
}

do_sys()
{
  cmd="ps"
  test_tools
  if [ "$check" -eq 1 ] ; then
    # Determine number of processes
    n=`ps -e | awk 'END { print NR-1 }'`
    PERFCLASS=System;PERFINSTANCE='_Total';PERFMETRIC=Processes;PERFINT=0;PERFSAMP=1;PERFAVG=$n;PERFMIN=$n;PERFMAX=$n;writeperf
  fi

  cmd="users"
  test_tools
  if [ "$check" -eq 1 ] ; then
    # Number of users, and load-averages
    n=`users | wc -w | tr -d ' '`
    PERFCLASS=System;PERFINSTANCE='_Total';PERFMETRIC='Active Logons';PERFINT=0;PERFSAMP=1;PERFAVG=$n;PERFMIN=$n;PERFMAX=$n;writeperf
  fi

  cmd="uptime"
  test_tools
  if [ "$check" -eq 1 ] ; then
    LINE=`uptime`
    set -- `expr "$LINE" : ".*average: \([^\n]*\)" | tr -d ,`
    PERFCLASS=System;PERFINSTANCE='_Total';PERFMETRIC='1 Minute Load Average';PERFINT=0;PERFSAMP=1;PERFAVG=$1;PERFMIN=$1;PERFMAX=$1;writeperf
    PERFCLASS=System;PERFINSTANCE='_Total';PERFMETRIC='5 Minute Load Average';PERFINT=0;PERFSAMP=1;PERFAVG=$2;PERFMIN=$2;PERFMAX=$2;writeperf
    PERFCLASS=System;PERFINSTANCE='_Total';PERFMETRIC='15 Minute Load Average';PERFINT=0;PERFSAMP=1;PERFAVG=$3;PERFMIN=$3;PERFMAX=$3;writeperf
  fi
}

hpux_swap()
{
  cmd="swapinfo"
  test_tools
  [ "$check" -eq 0 ] && return

	set -- `swapinfo -t | grep '^total' | tr -d %`

	PERFCLASS='Paging File';PERFINSTANCE='_Total';PERFMETRIC='Size KBytes';PERFINT=0;PERFSAMP=1;PERFAVG=$2;PERFMIN=$2;PERFMAX=$2;writeperf
	PERFCLASS='Paging File';PERFINSTANCE='_Total';PERFMETRIC='Used KBytes';PERFINT=0;PERFSAMP=1;PERFAVG=$3;PERFMIN=$3;PERFMAX=$3;writeperf
	PERFCLASS='Paging File';PERFINSTANCE='_Total';PERFMETRIC='Available KBytes';PERFINT=0;PERFSAMP=1;PERFAVG=$4;PERFMIN=$4;PERFMAX=$4;writeperf
	PERFCLASS='Paging File';PERFINSTANCE='_Total';PERFMETRIC='% Usage';PERFINT=0;PERFSAMP=1;PERFAVG=$5;PERFMIN=$5;PERFMAX=$5;writeperf
}

linux_swap()
{
	TotSize=0
	TotUsed=0

	grep '^/' /proc/swaps |
	( 
		while read FILE TYPE SIZE USED PRIOR ; do
			let TotSize=TotSize+SIZE
			let TotUsed=TotUsed+USED
		done

		PERFCLASS='Paging File';PERFINSTANCE='_Total';PERFMETRIC='Size KBytes';PERFINT=0;PERFSAMP=1;PERFAVG=$TotSize;PERFMIN=$TotSize;PERFMAX=$TotSize;writeperf
		PERFCLASS='Paging File';PERFINSTANCE='_Total';PERFMETRIC='Used KBytes';PERFINT=0;PERFSAMP=1;PERFAVG=$TotUsed;PERFMIN=$TotUsed;PERFMAX=$TotUsed;writeperf
		PERFCLASS='Paging File';PERFINSTANCE='_Total';PERFMETRIC='Available KBytes';PERFINT=0;PERFSAMP=1;PERFAVG=`expr $TotSize - $TotUsed`;PERFMIN=`expr $TotSize - $TotUsed`;PERFMAX=`expr $TotSize - $TotUsed`;writeperf
		if [ "$TotSize" -ne "0" ]; then
			PERFCLASS='Paging File';PERFINSTANCE='_Total';PERFMETRIC='% Usage';PERFINT=0;PERFSAMP=1
			PERFAVG=`awk -v a=$TotUsed -v b=$TotSize 'BEGIN { print a/b*100 }'`
			PERFMIN=$PERFAVG
			PERFMAX=$PERFAVG
			writeperf
		fi
	)
}

sun_swap()
{
  cmd="swap"
  test_tools
  [ "$check" -eq 0 ] && return

  set -- `swap -s | tr -d k | awk '{print $9, $11}'`
  TotUsed=$1
  TotSize=$2
  
  PERFCLASS='Paging File';PERFINSTANCE='_Total';PERFMETRIC='Size KBytes';PERFINT=0;PERFSAMP=1;PERFAVG=$TotSize;PERFMIN=$TotSize;PERFMAX=$TotSize;writeperf
  PERFCLASS='Paging File';PERFINSTANCE='_Total';PERFMETRIC='Used KBytes';PERFINT=0;PERFSAMP=1;PERFAVG=$TotUsed;PERFMIN=$TotUsed;PERFMAX=$TotUsed;writeperf
  PERFCLASS='Paging File';PERFINSTANCE='_Total';PERFMETRIC='Available KBytes';PERFINT=0;PERFSAMP=1;PERFAVG=`expr $TotSize - $TotUsed`;PERFMIN=`expr $TotSize - $TotUsed`;PERFMAX=`expr $TotSize - $TotUsed`;writeperf
  if [ $TotSize -ne 0 ]; then
		PERFCLASS='Paging File';PERFINSTANCE='_Total';PERFMETRIC='% Usage';PERFINT=0;PERFSAMP=1
		PERFAVG=`echo $TotUsed $TotSize | awk '{ print $1/$2*100 }'`
		PERFMIN=$PERFAVG
		PERFMAX=$PERFAVG
		writeperf
  fi
}

# Does not do sampling. Gets instantaneous value
aix_swap()
{
  cmd="lsps"
  test_tools
  [ "$check" -eq 0 ] && return

  lsps -s | 
  (
    read TITLE
    read TotSize TotUsed

    TotSize=`echo "$TotSize" | awk -F"MB" '{print $1 * 1024}'`
    TotUsed=`echo "$TotUsed" | awk -F"%" '{print $1}'`

    PERFCLASS='Paging File';PERFINSTANCE='_Total';PERFMETRIC='Size KBytes';PERFINT=0;PERFSAMP=1;PERFAVG=$TotSize;PERFMIN=$TotSize;PERFMAX=$TotSize;writeperf

    PERFCLASS='Paging File';PERFINSTANCE='_Total';PERFMETRIC='Used KBytes';PERFINT=0;PERFSAMP=1
    PERFAVG=`echo "$TotUsed * $TotSize / 100" | bc -l`;PERFMIN=$PERFAVG;PERFMAX=$PERFAVG;writeperf

    PERFCLASS='Paging File';PERFINSTANCE='_Total';PERFMETRIC='Available KBytes';PERFINT=0;PERFSAMP=1;PERFAVG=`echo "$TotSize - $PERFAVG" | bc -l`;PERFMIN=$PERFAVG;PERFMAX=$PERFAVG;writeperf
    PERFCLASS='Paging File';PERFINSTANCE='_Total';PERFMETRIC='% Usage';PERFINT=0;PERFSAMP=1;PERFAVG=$TotUsed;PERFMIN=$TotUsed;PERFMAX=$TotUsed;writeperf
  )
}

hpux_get_disk_transfers()
{
  cmd="vmstat"
  test_tools
  [ "$check" -eq 0 ] && return

  DISKTRANSFERS=0
  LINES=`vmstat -d | wc -l`
  LINES=`expr $LINES - 6`
  i=0
  ( while : ; do
      # First sample is an aggregate since boot time hence ignoring...
      vmstat -d $PerfDuration 2 | tail -$LINES
      i=`expr $i + 1`
      if [ $i -eq $PerfNumSamples ] ; then
        break
      fi
    done
    # Dummy data to force printing of final entry
    echo zzDummy 0
  ) | sort |
  ( # Process the data
    start=1
    while read DEVICE XFERRATE IGNORED ; do
      [ -z "$DEVICE" ] && continue
      if [ "$DEVICE"x != "$l_device"x ] ; then
        # Output the data for the last entry (if not empty)
        if [ -n "$l_device" ] ; then
          # Class,Inst,Metric,PerfNumSamples,interval,min,max,avg
          PERFCLASS=PhysicalDisk;PERFINSTANCE=$l_device;PERFMETRIC='Disk Transfers/sec';PERFINT=$PerfDuration;PERFSAMP=$PerfNumSamples;PERFAVG=`echo "$tot / $PerfNumSamples" | bc -l`;PERFMIN=$min;PERFMAX=$max;writeperf
          DISKTRANSFERS=`echo $DISKTRANSFERS + $tot | bc -l`
        fi
        start=1
        l_device=$DEVICE
      fi

      # This is the first line of info for this interface
      if [ "$start" -eq 1 ] ; then
        min=$XFERRATE
        max=$XFERRATE
        tot=$XFERRATE

        start=0
      else
        # Update total
        tot=`echo $tot + $XFERRATE | bc -l`

        # Update min value
        if [ "$XFERRATE" -lt "$min" ] ; then
          min=$XFERRATE
        fi

        # Update max value
        if [ "$XFERRATE" -gt "$max" ] ; then
          max=$XFERRATE
        fi
      fi
    done
    # The min and max values for the following counter are not calculated!!!
    PERFCLASS=PhysicalDisk;PERFINSTANCE='_Total';PERFMETRIC='Disk Transfers/sec';PERFINT=$PerfDuration;PERFSAMP=$PerfNumSamples;PERFAVG=`echo "$DISKTRANSFERS / $PerfNumSamples" | bc -l`;PERFMIN=$PERFAVG;PERFMAX=$PERFAVG;writeperf
  )
}

hpux_iostat()
{
  cmd="iostat"
  test_tools
  [ "$check" -eq 0 ] && return

  start=1
  l_name=""
  DISKKBYTES=0

  IOINTERVAL=$PerfDuration
  IOSAMPLES=$PerfNumSamples

  # Output fields are
  #  device    bps     sps    msps
  #  bps = Kilobytes transferred per second
  #  sps = Number of seeks per second
  # msps = Milliseconds per average seek

  ( iostat $IOINTERVAL $IOSAMPLES
    echo zzDummy 0 0 0
  ) | grep -v device | sort |
  while read DEVICE BPS SPS MSPS ; do
    [ -z "$DEVICE" ] && continue

    # Check if Device has changed
    if [ "$DEVICE"x != "$l_name"x ] ; then
      # Output the data for the last entry (if not empty)
      if [ -n "$l_name" ] ; then
        PERFCLASS=PhysicalDisk;PERFINSTANCE=$l_name;PERFMETRIC='Disk KBytes/sec';PERFINT=$IOINTERVAL;PERFSAMP=$IOSAMPLES;PERFAVG=`echo $btot / $IOSAMPLES | bc -l`;PERFMIN=$bmin;PERFMAX=$bmax;writeperf
        PERFCLASS=PhysicalDisk;PERFINSTANCE=$l_name;PERFMETRIC='Disk Bytes/sec';PERFINT=$IOINTERVAL;PERFSAMP=$IOSAMPLES;PERFAVG=`echo \( $btot \* 1024 \) / $IOSAMPLES | bc -l`;PERFMIN=`echo $bmin \* 1024 | bc -l`;PERFMAX=`echo $bmax \* 1024 | bc -l`;writeperf
        PERFCLASS=PhysicalDisk;PERFINSTANCE=$l_name;PERFMETRIC='Disk Seeks/sec';PERFINT=$IOINTERVAL;PERFSAMP=$IOSAMPLES;PERFAVG=`echo $stot / $IOSAMPLES | bc -l`;PERFMIN=$smin;PERFMAX=$smax;writeperf
        DISKKBYTES=`echo $DISKKBYTES + $btot | bc -l`
      fi
      start=1
      l_name=$DEVICE
    fi

    # This is the first line of info for this device
    if [ "$start" -eq 1 ] ; then
      bmin=$BPS
      bmax=$BPS
      btot=$BPS

      smin=$SPS
      smax=$SPS
      stot=$SPS

      start=0
    else
      # Update totals
      btot=`echo $btot + $BPS | bc -l`
      stot=`echo $stot + $SPS | bc -l`

      # Update min value
      tmp=`echo "if ($BPS < $bmin) 1; if ($BPS >= $bmin) 0" | bc -l`
      if [ "$tmp" -eq "1" ] ; then
	bmin=$BPS
      fi
      tmp=`echo "if ($SPS < $smin) 1; if ($SPS >= $smin) 0" | bc -l`
      if [ "$tmp" -eq "1" ] ; then
	smin=$SPS
      fi

      # Update max value
      tmp=`echo "if ($BPS > $bmax) 1; if ($BPS <= $bmax) 0" | bc -l`
      if [ "$tmp" -eq "1" ] ; then
	bmax=$BPS
      fi
      tmp=`echo "if ($SPS > $smax) 1; if ($SPS <= $smax) 0" | bc -l`
      if [ "$tmp" -eq "1" ] ; then
	smax=$SPS
      fi
    fi
  done

  # The min and max values for the following counter are not calculated!!!
  PERFCLASS=PhysicalDisk;PERFINSTANCE='_Total';PERFMETRIC='Disk Bytes/sec';PERFINT=$IOINTERVAL;PERFSAMP=$IOSAMPLES;PERFAVG=`echo \( $DISKKBYTES \* 1024 \) / $IOSAMPLES | bc -l`;PERFMIN=$PERFAVG;PERFMAX=$PERFAVG;writeperf
}

get_linux_scsi()
{
	DEVID=0
	( cat /proc/scsi/scsi
		echo Host: zzDummy Channel: 00 Id: 00 Lun: 00
	) |
	( # Read the "Attached devices:" line
		read LINE
		while read LINE ; do
			if expr "$LINE" : Host >/dev/null ; then
				# Print out the data for the previous disk
				if [ -n "$VENDOR" ] && ([ "$TYPE" = "Direct-Access" ] || [ "$TYPE" = "Enclosure" ]); then
					let DEVNUM=97+DEVID
					let DEVID=DEVID+1
					DEVNAME=`awk -v a=$DEVNUM 'BEGIN { printf("sd%c",a) }'`
					sdlist=$sdlist\|$DEVNAME
				fi
			else
				if expr "$LINE" : Vendor >/dev/null ; then
					VENDOR=`echo "$LINE" | sed -e 's/Vendor: *//' -e 's/ *Model.*//'`
				elif expr "$LINE" : Type >/dev/null ; then
					TYPE=`echo "$LINE" | sed -e 's/Type: *//' -e 's/ *ANSI.*//'`
				fi
			fi
		done
		echo $sdlist
	)
}

get_linux_disks()
{
   dlist=
	DSKLIST=`echo /proc/ide/ide*/hd*`
	if [ "$DSKLIST" != "/proc/ide/ide*/hd*" ]; then
		for DSK in $DSKLIST ; do
			dlist=$dlist\|`basename $DSK`
		done
	fi

	DSKLIST=`echo /proc/scsi/*/[0-9]*`
	if [ "$DSKLIST" != "/proc/scsi/*/[0-9]*" ]; then
		for DSK in $DSKLIST ; do
			DEVNAME=`echo $DSK | awk -F/ 'BEGIN { printf "sd%c", $NF+97 }'`
			dlist=$dlist\|$DEVNAME
		done
	fi
	if [ -e /proc/scsi/scsi ] ; then
		dlist=$dlist`get_linux_scsi`
	fi

	DRVLIST=`echo /proc/driver/cciss/cciss*`
	if [ "$DRVLIST" != "/proc/driver/cciss/cciss*" ]; then
		for DRV in $DRVLIST ; do
			DSKLIST=`grep cciss/ $DRV | awk -F : '{print $1}'`
			for DSK in $DSKLIST ; do
				dlist=$dlist\|$DSK
			done
		done
	fi

   if [ -n "$dlist" ]; then
      dlist=`echo $dlist | cut -d\| -f2-`
   fi
   echo $dlist
}

linux_iostat()
{
   IOINTERVAL=$PerfDuration
   IOSAMPLES=$PerfNumSamples
   let ACTUALIOSAMPLES=IOSAMPLES+1

   disk_time=0
   #Added for Disk Transfer/sec
   disk_reads=0
   disk_writes=0

   i=0
   ( while : ; do
      if [ -r /proc/diskstats ]; then
         DLIST=`get_linux_disks`
         egrep -w "$DLIST" /proc/diskstats
      else
         cat /proc/partitions
      fi

      i=`expr $i + 1`
      if [ $i -eq $ACTUALIOSAMPLES ] ; then
         break
      fi
      sleep $IOINTERVAL
   done
   # Dummy data to force printing of final entry
   echo 999 999 0 zzzz 0 0 0 0 0 0 0 0 0 0 0
   ) | grep -v '^major' | sort -n --key=1,2 |
   ( # Process the data
      tot_time=`cat /proc/uptime | awk '{print $1}'`
      start=0
      l_name=""
      NOTDONE="1"
      while [ "$NOTDONE" = "1" ] ; do
         NOTDONE="0"
         if [ -r /proc/diskstats ]; then
            if read MAJOR MINOR NAME RIO RMERGE RSECT RUSE WIO WMERGE WSECT WUSE CIO TIO IGNORED ; then
               NOTDONE="1"
            fi
         else
            if read MAJOR MINOR BLOCKS NAME RIO RMERGE RSECT RUSE WIO WMERGE WSECT WUSE CIO TIO IGNORED ; then
               NOTDONE="1"
            fi
         fi

         [ -z "$MAJOR" ] && continue

         if [ "$NAME"x != "$l_name"x ] ; then
            # Output the data for the last entry (if not empty)
            if [ -n "$l_name" ] ; then
               # Class,Inst,Metric,PerfNumSamples,interval,min,max,avg
               PERFCLASS=PhysicalDisk;PERFINSTANCE=$l_name;PERFMETRIC='Disk Total Reads';PERFINT=$IOINTERVAL;PERFSAMP=$IOSAMPLES;
               PERFAVG=`awk -v a=$riotot -v b=$IOSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`
               PERFMIN=$riomin;PERFMAX=$riomax;writeperf

               curr_disk_reads=$PERFAVG
			   disk_reads=`awk -v a=$disk_reads -v b=$curr_disk_reads 'BEGIN { print a+b }'`

               PERFCLASS=PhysicalDisk;PERFINSTANCE=$l_name;PERFMETRIC='Disk Reads/sec';PERFINT=$IOINTERVAL;PERFSAMP=$IOSAMPLES;
               PERFAVG=`awk -v a=$curr_disk_reads -v b=$tot_time 'BEGIN { printf("%.5f\n", a/b) }'`
               PERFMIN=$PERFAVG;PERFMAX=$PERFAVG;writeperf

               PERFCLASS=PhysicalDisk;PERFINSTANCE=$l_name;PERFMETRIC='Disk Total Writes';PERFINT=$IOINTERVAL;PERFSAMP=$IOSAMPLES;
               PERFAVG=`awk -v a=$wiotot -v b=$IOSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`
               PERFMIN=$wiomin;PERFMAX=$wiomax;writeperf

               curr_disk_writes=$PERFAVG
               disk_writes=`awk -v a=$disk_writes -v b=$curr_disk_writes 'BEGIN { print a+b }'`

               PERFCLASS=PhysicalDisk;PERFINSTANCE=$l_name;PERFMETRIC='Disk Writes/sec';PERFINT=$IOINTERVAL;PERFSAMP=$IOSAMPLES;
               PERFAVG=`awk -v a=$curr_disk_writes -v b=$tot_time 'BEGIN { printf("%.5f\n", a/b) }'`
               PERFMIN=$PERFAVG;PERFMAX=$PERFAVG;writeperf

               PERFCLASS=PhysicalDisk;PERFINSTANCE=$l_name;PERFMETRIC='Disk Transfers/sec';PERFINT=$IOINTERVAL;PERFSAMP=$IOSAMPLES
               PERFAVG=`awk -v a=$curr_disk_reads -v b=$curr_disk_writes -v c=$tot_time 'BEGIN { print (a+b)/c }'`
               PERFMIN=$PERFAVG;PERFMAX=$PERFAVG;writeperf

               PERFCLASS=PhysicalDisk;PERFINSTANCE=$l_name;PERFMETRIC='Disk Total Read Sectors';PERFINT=$IOINTERVAL;PERFSAMP=$IOSAMPLES;PERFAVG=`awk -v a=$rsecttot -v b=$IOSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;PERFMIN=$rsectmin;PERFMAX=$rsectmax;writeperf
               PERFCLASS=PhysicalDisk;PERFINSTANCE=$l_name;PERFMETRIC='Disk Total Write Sectors';PERFINT=$IOINTERVAL;PERFSAMP=$IOSAMPLES;PERFAVG=`awk -v a=$wsecttot -v b=$IOSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;PERFMIN=$wsectmin;PERFMAX=$wsectmax;writeperf
               PERFCLASS=PhysicalDisk;PERFINSTANCE=$l_name;PERFMETRIC='Disk Total Reads Merges';PERFINT=$IOINTERVAL;PERFSAMP=$IOSAMPLES;PERFAVG=`awk -v a=$rmergetot -v b=$IOSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;PERFMIN=$rmergemin;PERFMAX=$rmergemax;writeperf
               PERFCLASS=PhysicalDisk;PERFINSTANCE=$l_name;PERFMETRIC='Disk Total Write Merges';PERFINT=$IOINTERVAL;PERFSAMP=$IOSAMPLES;PERFAVG=`awk -v a=$wmergetot -v b=$IOSAMPLES 'BEGIN { printf("%.5f\n", a/b) }'`;PERFMIN=$wmergemin;PERFMAX=$wmergemax;writeperf

               if [ -n "$tio" ] ; then
                  PERFCLASS=PhysicalDisk;PERFINSTANCE=$l_name;PERFMETRIC='% Disk Time';PERFINT=$IOINTERVAL;PERFSAMP=$PerfNumSamples;PERFAVG=`awk -v a=$tiotot -v b=$PerfNumSamples 'BEGIN { printf("%.5f\n", a/b) }'`;PERFMIN=$tiomin;PERFMAX=$tiomax;writeperf
               fi
            fi
            start=1
            l_name=$NAME
         fi

         if [ -z "$RIO" ] ; then RIO=0 ; fi
         if [ -z "$RMERGE" ] ; then RMERGE=0 ; fi
         if [ -z "$RSECT" ] ; then RSECT=0 ; fi
         if [ -z "$WIO" ] ; then WIO=0 ; fi
         if [ -z "$WMERGE" ] ; then WMERGE=0 ; fi
         if [ -z "$WSECT" ] ; then WSECT=0 ; fi

         # This is the first line of info for this interface
         if [ "$start" -eq 1 ] ; then
            riomin=$RIO
            riomax=$RIO
            riotot=$RIO
            rmergemin=$RMERGE
            rmergemax=$RMERGE
            rmergetot=$RMERGE          
            rsectmin=$RSECT
            rsectmax=$RSECT
            rsecttot=$RSECT

            wiomin=$WIO
            wiomax=$WIO
            wiotot=$WIO
            wmergemin=$WMERGE
            wmergemax=$WMERGE
            wmergetot=$WMERGE          
            wsectmin=$WSECT
            wsectmax=$WSECT
            wsecttot=$WSECT

            prevTIO=$TIO
         elif [ "$start" -eq "$ACTUALIOSAMPLES" ] && [ -n "$TIO" ] ; then
            tio=`awk -v a=$TIO -v b=$prevTIO -v c=$IOINTERVAL 'BEGIN { print (a-b)/(c*10) }'`
            disk_time=`awk -v a=$TIO -v b=$prevTIO -v c=$disk_time 'BEGIN { print a-b+c }'`
            tiotot=`awk -v a=$tiotot -v b=$tio 'BEGIN {print a+b }'`
            tiomin=`awk -v a=$tiomin -v b=$tio 'BEGIN {if (b < a) print b; else print a}'`
            tiomax=`awk -v a=$tiomax -v b=$tio 'BEGIN {if (b > a) print b; else print a}'`
         else
            # Update totals
            let riotot=riotot+RIO
            let rmergetot=rmergetot+RMERGE
            let rsecttot=rsecttot+RSECT
            let wiotot=wiotot+WIO
            let wmergetot=wmergetot+WMERGE
            let wsecttot=wsecttot+WSECT

            # Update min value
            if [ "$RIO" -lt "$riomin" ] ; then
               riomin=$RIO
            fi
            if [ "$RMERGE" -lt "$rmergemin" ] ; then
               rmergemin=$RMERGE
            fi
            if [ "$RSECT" -lt "$rsectmin" ] ; then
               rsectmin=$RSECT
            fi
            if [ "$WIO" -lt "$wiomin" ] ; then
               wiomin=$WIO
            fi
            if [ "$WMERGE" -lt "$wmergemin" ] ; then
               wmergemin=$WMERGE
            fi
            if [ "$WSECT" -lt "$wsectmin" ] ; then
               wsectmin=$WSECT
            fi

            # Update max value
            if [ "$RIO" -gt "$riomax" ] ; then
               riomax=$RIO
            fi
            if [ "$RMERGE" -gt "$rmergemax" ] ; then
               rmergemax=$RMERGE
            fi
            if [ "$RSECT" -gt "$rsectmax" ] ; then
               rsectmax=$RSECT
            fi
            if [ "$WIO" -gt "$wiomax" ] ; then
               wiomax=$WIO
            fi
            if [ "$WMERGE" -gt "$wmergemin" ] ; then
               wmergemax=$WMERGE
            fi
            if [ "$WSECT" -gt "$wsectmax" ] ; then
               wsectmax=$WSECT
            fi

            if [ -n "$TIO" ] ; then
               tio=`awk -v a=$TIO -v b=$prevTIO -v c=$IOINTERVAL 'BEGIN { print (a-b)/(c*10) }'`
               disk_time=`awk -v a=$TIO -v b=$prevTIO -v c=$disk_time 'BEGIN { print a-b+c }'`
               prevTIO=$TIO
               if [ "$start" -eq 2 ] ; then
                  tiomin=$tio
                  tiomax=$tio
                  tiotot=$tio
               else
                  tiotot=`awk -v a=$tiotot -v b=$tio 'BEGIN {print a+b }'`
                  tiomin=`awk -v a=$tiomin -v b=$tio 'BEGIN {if (b < a) print b; else print a}'`
                  tiomax=`awk -v a=$tiomax -v b=$tio 'BEGIN {if (b > a) print b; else print a}'`
               fi
            fi
         fi
         let start=start+1
      done

      PERFCLASS=PhysicalDisk;PERFINSTANCE="_Total";PERFMETRIC='Disk Reads/sec';PERFINT=$IOINTERVAL;PERFSAMP=$IOSAMPLES
      PERFAVG=`awk -v a=$disk_reads -v b=$tot_time 'BEGIN { printf("%.5f\n", a/b) }'`
      PERFMIN=$PERFAVG;PERFMAX=$PERFAVG;writeperf

      PERFCLASS=PhysicalDisk;PERFINSTANCE="_Total";PERFMETRIC='Disk Writes/sec';PERFINT=$IOINTERVAL;PERFSAMP=$IOSAMPLES
      PERFAVG=`awk -v a=$disk_writes -v b=$tot_time 'BEGIN { printf("%.5f\n", a/b) }'`
      PERFMIN=$PERFAVG;PERFMAX=$PERFAVG;writeperf

      PERFCLASS=PhysicalDisk;PERFINSTANCE="_Total";PERFMETRIC='Disk Transfers/sec';PERFINT=$IOINTERVAL;PERFSAMP=$IOSAMPLES
      PERFAVG=`awk -v a=$disk_reads -v b=$disk_writes -v c=$tot_time 'BEGIN { print (a+b)/c }'`
      PERFMIN=$PERFAVG;PERFMAX=$PERFAVG;writeperf

      if [ -n "$tio" ] ; then
         PERFCLASS=PhysicalDisk;PERFINSTANCE="_Total";PERFMETRIC='% Disk Time';PERFINT=$IOINTERVAL;PERFSAMP=$IOSAMPLES
         PERFAVG=`awk -v a=$disk_time -v b=$IOSAMPLES -v c=$IOINTERVAL 'BEGIN { print a/(b*c*10) }'`
         PERFMIN=$PERFAVG;PERFMAX=$PERFAVG;writeperf
      fi
   )
}

sun_iostat()
{
  cmd="iostat"
  test_tools
  [ "$check" -eq 0 ] && return

  start=1
  l_name=""
  DISKTRANSFERS=0
  DISKKBYTES=0
  DISKTIME=0

  IOINTERVAL=$PerfDuration
  IOSAMPLES=`expr $PerfNumSamples + 1`
  ACTUALIOSAMPLES=$PerfNumSamples

  # Output fields are
  # device       r/s    w/s   kr/s   kw/s wait actv  svc_t  %w  %b
  #
  (
    iostat -x $IOINTERVAL $IOSAMPLES
    echo zzDummy 0 0 0 0 0 0 0 0 0
  ) | grep -v device | awk 'BEGIN { FS = " "; } { if ( NR == 1 ) { dev_f = $1; } if ( NR > 1 ) { dev_a = $1; } if ( dev_f == dev_a ) flag = 1; if ( flag ) print $0; }' | sort |
  (
    while read DEVICE RS WS KRS KWS WT ACTV SVCT PW PB IGNORED ; do
      [ -z "$DEVICE" ] && continue

      # Check if Device has changed
      if [ "$DEVICE"x != "$l_name"x ] ; then
        # Output the data for the last entry (if not empty)
        if [ -n "$l_name" ] ; then
	  # Class,Inst,Metric,PerfNumSamples,interval,min,max,avg
          PERFCLASS=PhysicalDisk;PERFINSTANCE=$l_name;PERFMETRIC='Disk Reads/sec';PERFINT=$IOINTERVAL;PERFSAMP=$ACTUALIOSAMPLES;PERFAVG=`echo $rstot / $ACTUALIOSAMPLES | bc -l`;PERFMIN=$rsmin;PERFMAX=$rsmax;writeperf
          PERFCLASS=PhysicalDisk;PERFINSTANCE=$l_name;PERFMETRIC='Disk Writes/sec';PERFINT=$IOINTERVAL;PERFSAMP=$ACTUALIOSAMPLES;PERFAVG=`echo $wstot / $ACTUALIOSAMPLES | bc -l`;PERFMIN=$wsmin;PERFMAX=$wsmax;writeperf
          PERFCLASS=PhysicalDisk;PERFINSTANCE=$l_name;PERFMETRIC='Disk Transfers/sec';PERFINT=$IOINTERVAL;PERFSAMP=$ACTUALIOSAMPLES;PERFAVG=`echo $stot / $ACTUALIOSAMPLES | bc -l`;PERFMIN=$smin;PERFMAX=$smax;writeperf
          DISKTRANSFERS=`echo $DISKTRANSFERS + $stot | bc -l`
          PERFCLASS=PhysicalDisk;PERFINSTANCE=$l_name;PERFMETRIC='Disk Read KBytes/sec';PERFINT=$IOINTERVAL;PERFSAMP=$ACTUALIOSAMPLES;PERFAVG=`echo $krstot / $ACTUALIOSAMPLES | bc -l`;PERFMIN=$krsmin;PERFMAX=$krsmax;writeperf
          PERFCLASS=PhysicalDisk;PERFINSTANCE=$l_name;PERFMETRIC='Disk Write KBytes/sec';PERFINT=$IOINTERVAL;PERFSAMP=$ACTUALIOSAMPLES;PERFAVG=`echo $kwstot / $ACTUALIOSAMPLES | bc -l`;PERFMIN=$kwsmin;PERFMAX=$kwsmax;writeperf
          PERFCLASS=PhysicalDisk;PERFINSTANCE=$l_name;PERFMETRIC='Disk Bytes/sec';PERFINT=$IOINTERVAL;PERFSAMP=$ACTUALIOSAMPLES;PERFAVG=`echo \( $kstot \* 1024 \) / $ACTUALIOSAMPLES | bc -l`;PERFMIN=`echo $ksmin \* 1024 | bc -l`;PERFMAX=`echo $ksmax \* 1024 | bc -l`;writeperf
          DISKKBYTES=`echo $DISKKBYTES + $kstot | bc -l`
          if [ -n "$pbtot" ] ; then
            PERFCLASS=PhysicalDisk;PERFINSTANCE=$l_name;PERFMETRIC='% Disk Time';PERFINT=$IOINTERVAL;PERFSAMP=$ACTUALIOSAMPLES;PERFAVG=`echo $pbtot / $ACTUALIOSAMPLES | bc -l`;PERFMIN=$pbmin;PERFMAX=$pbmax;writeperf
            DISKTIME=`echo $DISKTIME + $pbtot | bc -l`
          fi
        fi
        start=1
        l_name=$DEVICE
      fi

      # This is the first line of info for this device
      if [ "$start" -eq 1 ] ; then
        rsmin=$RS
        rsmax=$RS
        rstot=$RS

        wsmin=$WS
        wsmax=$WS
        wstot=$WS

        smin=`echo $WS + $RS | bc -l`
        smax=$smin
        stot=$smin

        krsmin=$KRS
        krsmax=$KRS
        krstot=$KRS

        kwsmin=$KWS
        kwsmax=$KWS
        kwstot=$KWS

        ksmin=`echo $KRS + $KWS | bc -l`
        ksmax=$ksmin
        kstot=$ksmin

	pbmin=$PB
	pbmax=$PB
	pbtot=$PB

        start=0
      else
        # Update totals
        rstot=`echo $rstot + $RS | bc -l`
        wstot=`echo $wstot + $WS | bc -l`
        stot=`echo $wstot + $rstot | bc -l`
        krstot=`echo $krstot + $KRS | bc -l`
        kwstot=`echo $kwstot + $KWS | bc -l`
        kstot=`echo $krstot + $kwstot | bc -l`

        # Update min value
        tmp=`echo "if ($RS < $rsmin) 1; if ($RS >= $rsmin) 0" | bc -l`
        if [ "$tmp" -eq "1" ] ; then
          rsmin=$RS
        fi
        tmp=`echo "if ($WS < $wsmin) 1; if ($WS >= $wsmin) 0" | bc -l`
        if [ "$tmp" -eq "1" ] ; then
          wsmin=$WS
        fi
        tmp=`echo "if ($WS + $RS < $smin) 1; if ($WS + $RS >= $smin) 0" | bc -l`
        if [ "$tmp" -eq "1" ] ; then
          smin=`echo $WS + $RS | bc -l`
        fi
        tmp=`echo "if ($KRS < $krsmin) 1; if ($KRS >= $krsmin) 0" | bc -l`
        if [ "$tmp" -eq "1" ] ; then
          krsmin=$KRS
        fi
        tmp=`echo "if ($KWS < $kwsmin) 1; if ($KWS >= $kwsmin) 0" | bc -l`
        if [ "$tmp" -eq "1" ] ; then
          kwsmin=$KWS
        fi
        tmp=`echo "if ($KWS + $KRS < $ksmin) 1; if ($KWS + $KRS >= $ksmin) 0" | bc -l`
        if [ "$tmp" -eq "1" ] ; then
          ksmin=`echo $KWS + $KRS | bc -l`
        fi

        # Update max value
        tmp=`echo "if ($RS > $rsmax) 1; if ($RS <= $rsmax) 0" | bc -l`
        if [ "$tmp" -eq "1" ] ; then
          rsmax=$RS
        fi
        tmp=`echo "if ($WS > $wsmax) 1; if ($WS <= $wsmax) 0" | bc -l`
        if [ "$tmp" -eq "1" ] ; then
          wsmax=$WS
        fi
        tmp=`echo "if ($WS + $RS > $smax) 1; if ($WS + $RS <= $smax) 0" | bc -l`
        if [ "$tmp" -eq "1" ] ; then
          smax=`echo $WS + $RS | bc -l`
        fi
        tmp=`echo "if ($KRS > $krsmax) 1; if ($KRS <= $krsmax) 0" | bc -l`
        if [ "$tmp" -eq "1" ] ; then
          krsmax=$KRS
        fi
        tmp=`echo "if ($KWS > $kwsmax) 1; if ($KWS <= $kwsmax) 0" | bc -l`
        if [ "$tmp" -eq "1" ] ; then
          kwsmax=$KWS
        fi
        tmp=`echo "if ($KWS + $KRS > $ksmax) 1; if ($KWS + $KRS <= $ksmax) 0" | bc -l`
        if [ "$tmp" -eq "1" ] ; then
          ksmax=`echo $KWS + $KRS | bc -l`
        fi

        if [ -n "$PB" ] ; then
          pbtot=`echo $pbtot + $PB | bc -l`
          tmp=`echo "if ($PB < $pbmin) 1; if ($PB >= $pbmin) 0" | bc -l`
          if [ "$tmp" -eq "1" ] ; then
            pbmin=$PB
          fi
          tmp=`echo "if ($PB > $pbmax) 1; if ($PB <= $pbmax) 0" | bc -l`
          if [ "$tmp" -eq "1" ] ; then
            pbmax=$PB
          fi
        fi
      fi
    done

    # The min and max values for the following counters are not calculated!!!
    PERFCLASS=PhysicalDisk;PERFINSTANCE='_Total';PERFMETRIC='Disk Transfers/sec';PERFINT=$IOINTERVAL;PERFSAMP=$ACTUALIOSAMPLES;PERFAVG=`echo $DISKTRANSFERS / $ACTUALIOSAMPLES | bc -l`;PERFMIN=$PERFAVG;PERFMAX=$PERFAVG;writeperf
    PERFCLASS=PhysicalDisk;PERFINSTANCE='_Total';PERFMETRIC='Disk Bytes/sec';PERFINT=$IOINTERVAL;PERFSAMP=$ACTUALIOSAMPLES;PERFAVG=`echo \( $DISKKBYTES \* 1024 \) / $ACTUALIOSAMPLES | bc -l`;PERFMIN=$PERFAVG;PERFMAX=$PERFAVG;writeperf
    if [ -n "$pbtot" ] ; then
      PERFCLASS=PhysicalDisk;PERFINSTANCE='_Total';PERFMETRIC='% Disk Time';PERFINT=$IOINTERVAL;PERFSAMP=$ACTUALIOSAMPLES;PERFAVG=`echo $DISKTIME / $ACTUALIOSAMPLES | bc -l`;PERFMIN=$PERFAVG;PERFMAX=$PERFAVG;writeperf
    fi
  )
}

aix_iostat()
{
  cmd="iostat"
  test_tools
  [ "$check" -eq 0 ] && return

  start=1
  l_name=""
  DISKTRANSFERS=0
  DISKKBYTES=0

  IOINTERVAL=$PerfDuration
  ACTUALIOSAMPLES=`expr $PerfNumSamples + 1`

  REQOP=`iostat -d $IOINTERVAL $ACTUALIOSAMPLES`

  IGNORED=`echo "$REQOP" | grep " Disk history since boot not available. "`
  if [ -n "$IGNORED" ] ; then
      REQOP=`echo "$REQOP" | grep -v " Disk history since boot not available. "`
  fi
  REQOP=`echo "$REQOP" | awk '{if($1 == "Disks:")flag=1;if(flag)print $0;}'`
  if [ -z "$IGNORED" ]; then
      REQOP=`echo "$REQOP" | awk 'BEGIN { FS = " "; } { if (NR == 2 ) { dev_f = $1; } if ( NR > 2 ) { dev_a = $1; if ( dev_f == dev_a ) flag = 1;if ( flag ) print $0; }}'`
  fi
  REQOP=`echo "$REQOP" | grep -v "^Disks:"`

  # First sample is an aggregate since boot time hence ignoring...
  ACTUALIOSAMPLES=$PerfNumSamples

  (
    echo "$REQOP" 
    echo zzDummy 0 0 0 0 0 
  ) | sort |
  (
    while read DEVICE UNUSED KBPS TPS KRS KWS IGNORED ; do
      [ -z "$DEVICE" ] && continue

      # Check if Device has changed
      if [ "$DEVICE"x != "$l_name"x ] ; then
        # Output the data for the last entry (if not empty)
        if [ -n "$l_name" ] ; then
	    # Class,Inst,Metric,PerfNumSamples,interval,min,max,avg
          PERFCLASS=PhysicalDisk;PERFINSTANCE=$l_name;PERFMETRIC='Disk Bytes/sec';PERFINT=$IOINTERVAL;PERFSAMP=$ACTUALIOSAMPLES;PERFAVG=`echo \( $kstot \* 1024 \) / $ACTUALIOSAMPLES | bc -l`;PERFMIN=`echo $ksmin \* 1024 | bc -l`;PERFMAX=`echo $ksmax \* 1024 | bc -l`;writeperf
          DISKKBYTES=`echo $DISKKBYTES + $kstot | bc -l`
          PERFCLASS=PhysicalDisk;PERFINSTANCE=$l_name;PERFMETRIC='Disk Transfers/sec';PERFINT=$IOINTERVAL;PERFSAMP=$ACTUALIOSAMPLES;PERFAVG=`echo $stot / $ACTUALIOSAMPLES | bc -l`;PERFMIN=$smin;PERFMAX=$smax;writeperf
          DISKTRANSFERS=`echo $DISKTRANSFERS + $stot | bc -l`
          PERFCLASS=PhysicalDisk;PERFINSTANCE=$l_name;PERFMETRIC='Disk Total Reads (KB)';PERFINT=$IOINTERVAL;PERFSAMP=$ACTUALIOSAMPLES;PERFAVG=`echo $krstot / $ACTUALIOSAMPLES | bc -l`;PERFMIN=$krsmin;PERFMAX=$krsmax;writeperf
          PERFCLASS=PhysicalDisk;PERFINSTANCE=$l_name;PERFMETRIC='Disk Total Writes (KB)';PERFINT=$IOINTERVAL;PERFSAMP=$ACTUALIOSAMPLES;PERFAVG=`echo $kwstot / $ACTUALIOSAMPLES | bc -l`;PERFMIN=$kwsmin;PERFMAX=$kwsmax;writeperf
        fi
        start=1
        l_name=$DEVICE
      fi

      # This is the first line of info for this device
      if [ "$start" -eq 1 ] ; then
        ksmin=$KBPS
        ksmax=$KBPS
        kstot=$KBPS

        smin=$TPS
        smax=$TPS
        stot=$TPS

        krsmin=$KRS
        krsmax=$KRS
        krstot=$KRS

        kwsmin=$KWS
        kwsmax=$KWS
        kwstot=$KWS

        start=0
      else
        # Update totals
        kstot=`echo $kstot + $KBPS | bc -l`
        stot=`echo $stot + $TPS | bc -l`
        krstot=`echo $krstot + $KRS | bc -l`
        kwstot=`echo $kwstot + $KWS | bc -l`

        # Update min value
        tmp=`echo "if ($KBPS < $ksmin) 1; if ($KBPS >= $ksmin) 0" | bc -l`
        if [ "$tmp" -eq "1" ] ; then
          ksmin=$KBPS
        fi
        tmp=`echo "if ($TPS < $smin) 1; if ($TPS >= $smin) 0" | bc -l`
        if [ "$tmp" -eq "1" ] ; then
          smin=$TPS
        fi
        tmp=`echo "if ($KRS < $krsmin) 1; if ($KRS >= $krsmin) 0" | bc -l`
        if [ "$tmp" -eq "1" ] ; then
          krsmin=$KRS
        fi
        tmp=`echo "if ($KWS < $kwsmin) 1; if ($KWS >= $kwsmin) 0" | bc -l`
        if [ "$tmp" -eq "1" ] ; then
          kwsmin=$KWS
        fi

        # Update max value
        tmp=`echo "if ($KBPS > $ksmax) 1; if ($KBPS <= $ksmax) 0" | bc -l`
        if [ "$tmp" -eq "1" ] ; then
          ksmax=$KBPS
        fi
        tmp=`echo "if ($TPS > $smax) 1; if ($TPS <= $smax) 0" | bc -l`
        if [ "$tmp" -eq "1" ] ; then
          smax=$TPS
        fi
        tmp=`echo "if ($KRS > $krsmax) 1; if ($KRS <= $krsmax) 0" | bc -l`
        if [ "$tmp" -eq "1" ] ; then
          krsmax=$KRS
        fi
        tmp=`echo "if ($KWS > $kwsmax) 1; if ($KWS <= $kwsmax) 0" | bc -l`
        if [ "$tmp" -eq "1" ] ; then
          kwsmax=$KWS
        fi
      fi
    done

    # The min and max values for the following two counters are not calculated!!!
    PERFCLASS=PhysicalDisk;PERFINSTANCE='_Total';PERFMETRIC='Disk Bytes/sec';PERFINT=$IOINTERVAL;PERFSAMP=$ACTUALIOSAMPLES;PERFAVG=`echo \( $DISKKBYTES \* 1024 \) / $ACTUALIOSAMPLES | bc -l`;PERFMIN=$PERFAVG;PERFMAX=$PERFAVG;writeperf
    PERFCLASS=PhysicalDisk;PERFINSTANCE='_Total';PERFMETRIC='Disk Transfers/sec';PERFINT=$IOINTERVAL;PERFSAMP=$ACTUALIOSAMPLES;PERFAVG=`echo $DISKTRANSFERS / $ACTUALIOSAMPLES | bc -l`;PERFMIN=$PERFAVG;PERFMAX=$PERFAVG;writeperf
  )
}

hpuxsun_netstat()
{
  cmd="netstat"
  test_tools
  [ "$check" -eq 0 ] && return

  NSINTERVAL=$PerfDuration
  NSSAMPLES=$PerfNumSamples

  # Get all the data, strip the headers, sort it so that all lines for the same
  # interfaces appear together
  i=0
  ( while : ; do
      netstat -ni
      i=`expr $i + 1`
      if [ $i -eq $NSSAMPLES ] ; then
        break
      fi
      sleep $NSINTERVAL
    done
    # Dummy data to force printing of final entry
    echo zzDummy 0 0 0 0 0 0 0 0
  ) | grep -v '^Name' | sort |
  ( # Process the data
    start=1
    l_name=""
    l_ipaddr=""

    while read NAME MTU NWK ADDR IPKTS IERRS OPKTS IGNORED ; do
      # On PA-RISC there are fewer fields
      # HPUX PA-RISC:
      #   Name      Mtu Network          Address         Ipkts         Opkts
      # HPUX IA64:
      #   Name      Mtu  Network         Address         Ipkts   Ierrs Opkts   Oerrs Coll
      # SUN has an extra field, which is ignored
      #   Name  Mtu  Net/Dest      Address        Ipkts  Ierrs Opkts  Oerrs Collis Queue
      #
      [ -z "$NAME" ] && continue
      [ -z "$OPKTS" ] && OPKTS=$IERRS

      # Check if Name has changed
      if [ "$NAME"x != "$l_name"x ] ; then
        # Output the data for the last entry (if not empty)
        if [ -n "$l_name" ] ; then
          # Class,Inst,Metric,PerfNumSamples,interval,min,max,avg
          PERFCLASS='Network Interface';PERFINSTANCE=$l_name;PERFMETRIC='Packets Received';PERFINT=$NSINTERVAL;PERFSAMP=$NSSAMPLES;PERFAVG=`echo $itot / $NSSAMPLES | bc -l`;PERFMIN=$imin;PERFMAX=$imax;writeperf
          PERFCLASS='Network Interface';PERFINSTANCE=$l_name;PERFMETRIC='Packets Sent';PERFINT=$NSINTERVAL;PERFSAMP=$NSSAMPLES;PERFAVG=`echo $otot / $NSSAMPLES | bc -l`;PERFMIN=$omin;PERFMAX=$omax;writeperf
        fi
        start=1
        l_name=$NAME
        l_ipaddr=$ADDR
      fi

      # This is the first line of info for this interface
      if [ "$start" -eq 1 ] ; then
        imin=$IPKTS
        imax=$IPKTS
        itot=$IPKTS

        omin=$OPKTS
        omax=$OPKTS
        otot=$OPKTS

        start=0
      else
        # Update totals
        itot=`echo $itot + $IPKTS | bc -l`
        otot=`echo $otot + $OPKTS | bc -l`

        # Update min value
        if [ "$IPKTS" -lt "$imin" ] ; then
         imin=$IPKTS
        fi
        if [ "$OPKTS" -lt "$omin" ] ; then
          omin=$OPKTS
        fi

        # Update max value
        if [ "$IPKTS" -gt "$imax" ] ; then
        imax=$IPKTS
        fi
        if [ "$OPKTS" -gt "$omax" ] ; then
          omax=$OPKTS
        fi
      fi
    done
  )
}

sun_get_network_transfers()
{
  cmd="netstat"
  test_tools
  [ "$check" -eq 0 ] && return

  NSINTERVAL=$PerfDuration
  NSSAMPLES=$PerfNumSamples
  TOT_BYTES_PER_SEC=0

  (
    netstat -ni
  ) | grep -v '^Name' |
  ( 
    while read NAME IGNORED ; do
      [ -z "$NAME" ] && continue

      OS_REV=`uname -r | awk -F. '{print $2}'`
      i=0
      start=1
      while : ; do
        if [ "$OS_REV" -le 9 ] ; then
          FIND="/^$NAME/,/^$/"
          TEMP=`netstat -k | awk $FIND`
          RBYTES64=`expr "$TEMP" : '.*rbytes64 *\([^ ]*\)'`
          OBYTES64=`expr "$TEMP" : '.*obytes64 *\([^ ]*\)'`
          [ \( -z "$RBYTES64" \) -o \( -z "$OBYTES64" \) ] && break
        else
          FIND="*:*:$NAME:*bytes64"
          TEMP=`kstat -p $FIND`
          [ -z "$TEMP" ] && break
          OBYTES64=`echo "$TEMP" | grep obytes64 | awk '{print $2}'`
          RBYTES64=`echo "$TEMP" | grep rbytes64 | awk '{print $2}'`
        fi

        if [ "$start" -eq 1 ] ; then
          # This is the first line of info for this interface
          bytes=`expr $RBYTES64 + $OBYTES64`
          bytesmin=$bytes
          bytesmax=0
          bytestot=0
          start=0
        else
          bytes_per_sec=`expr $RBYTES64 + $OBYTES64 - $bytes`
          bytes=`expr $bytes + $bytes_per_sec`

          # Update totals
          bytestot=`echo $bytestot + $bytes_per_sec | bc -l`

          # Update min value
          if [ "$bytes_per_sec" -lt "$bytesmin" ] ; then
            bytesmin=$bytes_per_sec
          fi
          # Update max value
          if [ "$bytes_per_sec" -gt "$bytesmax" ] ; then
            bytesmax=$bytes_per_sec
          fi
        fi
        i=`expr $i + 1`
        if [ $i -gt $NSSAMPLES ] ; then
          PERFCLASS='Network Interface';PERFINSTANCE=$NAME;PERFMETRIC='Bytes Total/sec';PERFINT=$NSINTERVAL;PERFSAMP=$NSSAMPLES;PERFAVG=`echo $bytestot / \( $NSSAMPLES \* $NSINTERVAL \) | bc -l`;PERFMIN=`echo $bytesmin / $NSINTERVAL | bc -l`;PERFMAX=`echo $bytesmax / $NSINTERVAL | bc -l`;writeperf
          TOT_BYTES_PER_SEC=`expr $TOT_BYTES_PER_SEC + $bytestot`
          break
        fi
        sleep $NSINTERVAL
      done
    done
    # The min and max values for the following counter are not calculated!!!
    PERFCLASS='Server';PERFINSTANCE='_Total';PERFMETRIC='Bytes Total/sec';PERFINT=$NSINTERVAL;PERFSAMP=$NSSAMPLES;PERFAVG=`echo $TOT_BYTES_PER_SEC / \( $NSSAMPLES \* $NSINTERVAL \) | bc -l`;PERFMIN=$PERFAVG;PERFMAX=$PERFAVG;writeperf
  )
}

hpux_get_network_transfers()
{
  cmd="lanscan"
  test_tools
  [ "$check" -eq 0 ] && return

  OS_VER=`uname -r | awk -F. '{print $2}'`
  if [ "$OS_VER" -eq 11 ] ; then
    LANCODE="ppa"
  else
    OS_REV=`uname -r | awk -F. '{print $3}'`
    if [ \( "$OS_VER" -eq 10 \) -a \( "$OS_REV" -eq 20 \) ] ; then
      LANCODE="nmid"
    else
      # Non-supported OS version
      return
    fi
  fi
  NSINTERVAL=$PerfDuration
  NSSAMPLES=$PerfNumSamples
  TOT_BYTES_PER_SEC=0
  lanscan -p |
  (
    while read LANID ; do
      i=0
      start=1
      while : ; do
        TEMP=`echo "lan \012 $LANCODE $LANID \012 display \012" | lanadmin 2>/dev/null`

        IBYTES=`echo "$TEMP" | grep "Inbound Octets" | awk '{print $4}'`
        OBYTES=`echo "$TEMP" | grep "Outbound Octets" | awk '{print $4}'`

        if [ "$start" -eq 1 ] ; then
          # This is the first line of info for this interface
          BYTES=`expr $IBYTES + $OBYTES`
          bytesmin=$BYTES
          bytesmax=0
          bytestot=0
          start=0
        else
          BYTES_PER_SEC=`expr $IBYTES + $OBYTES - $BYTES`
          BYTES=`expr $BYTES + $BYTES_PER_SEC`

          # Update totals
          bytestot=`echo $bytestot + $BYTES_PER_SEC | bc -l`

          # Update min value
          tmp=`echo "if ($BYTES_PER_SEC < $bytesmin) 1; if ($BYTES_PER_SEC >= $bytesmin) 0" | bc -l`
          if [ "$tmp" -eq "1" ] ; then
            bytesmin=$BYTES_PER_SEC
          fi
          # Update max value
          if [ "$BYTES_PER_SEC" -gt "$bytesmax" ] ; then
            bytesmax=$BYTES_PER_SEC
          fi
        fi
        i=`expr $i + 1`
        if [ $i -gt $NSSAMPLES ] ; then
          PERFCLASS='Network Interface';PERFINSTANCE=$LANID;PERFMETRIC='Bytes Total/sec';PERFINT=$NSINTERVAL;PERFSAMP=$NSSAMPLES;PERFAVG=`echo $bytestot / $NSSAMPLES | bc -l`;PERFMIN=$bytesmin;PERFMAX=$bytesmax;writeperf
          TOT_BYTES_PER_SEC=`echo $TOT_BYTES_PER_SEC + $bytestot | bc -l`
          break
        fi
        sleep $NSINTERVAL
      done
    done
    # The min and max values for the following counter are not calculated!!!
    PERFCLASS='Server';PERFINSTANCE='_Total';PERFMETRIC='Bytes Total/sec';PERFINT=$NSINTERVAL;PERFSAMP=$NSSAMPLES;PERFAVG=`echo $TOT_BYTES_PER_SEC / $NSSAMPLES | bc -l`;PERFMIN=$PERFAVG;PERFMAX=$PERFAVG;writeperf
  )
}

linux_netstat()
{
	NSINTERVAL=$PerfDuration
	NSSAMPLES=$PerfNumSamples

	# Get all the data, strip the headers, sort it so that all lines for the same
	# interfaces appear together
	i=0
	( while : ; do
		grep -v 'No statistics' /proc/net/dev | grep ':' | tr : ' '
		let i=i+1
		if [ $i -eq $NSSAMPLES ] ; then
			break
		fi
		sleep $NSINTERVAL
	done
	# Dummy data to force printing of final entry
	echo zzDummy 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
	) | sort |
	( # Process the data
		l_name=""
		iraw=
		imin=
		imax=
		itot=

		oraw=
		omin=
		omax=
		otot=

		traw=
		tmin=
		tmax=
		ttot=

		ibraw=
		ibmin=
		ibmax=
		ibtot=

		obraw=
		obmin=
		obmax=
		obtot=

		tbraw=
		tbmin=
		tbmax=
		tbtot=

		TOT_BYTES=0

		while read NAME IBYTES IPKTS IERRS IDROP IFIFO IFRAME ICOMP IMCST OBYTES OPKTS IGNORED ; do
			# Check if Name has changed
			if [ "$NAME"x != "$l_name"x ] ; then
				# Output the data for the last entry (if not empty)
				if [ -n "$l_name" ] ; then
					# Class,Inst,Metric,PerfNumSamples,interval,min,max,avg
					PERFCLASS='Network Interface';PERFINSTANCE=$l_name;PERFMETRIC='Packets Received/sec';PERFINT=$NSINTERVAL;PERFSAMP=$NSSAMPLES
					PERFAVG=`awk -v a=$itot -v b=$NSSAMPLES -v c=$NSINTERVAL 'BEGIN { print a/(b*c) }'`;
					PERFMIN=`awk -v a=$imin -v b=$NSINTERVAL 'BEGIN { printf("%.5f\n", a/b) }'`;
					PERFMAX=`awk -v a=$imax -v b=$NSINTERVAL 'BEGIN { printf("%.5f\n", a/b) }'`;
					writeperf

					PERFCLASS='Network Interface';PERFINSTANCE=$l_name;PERFMETRIC='Packets Sent/sec';PERFINT=$NSINTERVAL;PERFSAMP=$NSSAMPLES
					PERFAVG=`awk -v a=$otot -v b=$NSSAMPLES -v c=$NSINTERVAL 'BEGIN { print a/(b*c) }'`
					PERFMIN=`awk -v a=$omin -v b=$NSINTERVAL 'BEGIN { printf("%.5f\n", a/b) }'`;
					PERFMAX=`awk -v a=$omax -v b=$NSINTERVAL 'BEGIN { printf("%.5f\n", a/b) }'`;
					writeperf
			
					PERFCLASS='Network Interface';PERFINSTANCE=$l_name;PERFMETRIC='Packets Total/sec';PERFINT=$NSINTERVAL;PERFSAMP=$NSSAMPLES
					PERFAVG=`awk -v a=$ttot -v b=$NSSAMPLES -v c=$NSINTERVAL 'BEGIN { print a/(b*c) }'`
					PERFMIN=`awk -v a=$tmin -v b=$NSINTERVAL 'BEGIN { printf("%.5f\n", a/b) }'`;
					PERFMAX=`awk -v a=$tmax -v b=$NSINTERVAL 'BEGIN { printf("%.5f\n", a/b) }'`;
					writeperf

					PERFCLASS='Network Interface';PERFINSTANCE=$l_name;PERFMETRIC='Bytes Received/sec';PERFINT=$NSINTERVAL;PERFSAMP=$NSSAMPLES
					PERFAVG=`awk -v a=$ibtot -v b=$NSSAMPLES -v c=$NSINTERVAL 'BEGIN { print a/(b*c) }'`;
					PERFMIN=`awk -v a=$ibmin -v b=$NSINTERVAL 'BEGIN { printf("%.5f\n", a/b) }'`;
					PERFMAX=`awk -v a=$ibmax -v b=$NSINTERVAL 'BEGIN { printf("%.5f\n", a/b) }'`;
					writeperf

					PERFCLASS='Network Interface';PERFINSTANCE=$l_name;PERFMETRIC='Bytes Sent/sec';PERFINT=$NSINTERVAL;PERFSAMP=$NSSAMPLES
					PERFAVG=`awk -v a=$obtot -v b=$NSSAMPLES -v c=$NSINTERVAL 'BEGIN { print a/(b*c) }'`
					PERFMIN=`awk -v a=$obmin -v b=$NSINTERVAL 'BEGIN { printf("%.5f\n", a/b) }'`;
					PERFMAX=`awk -v a=$obmax -v b=$NSINTERVAL 'BEGIN { printf("%.5f\n", a/b) }'`;
					writeperf
			
					PERFCLASS='Network Interface';PERFINSTANCE=$l_name;PERFMETRIC='Bytes Total/sec';PERFINT=$NSINTERVAL;PERFSAMP=$NSSAMPLES
					PERFAVG=`awk -v a=$tbtot -v b=$NSSAMPLES -v c=$NSINTERVAL 'BEGIN { print a/(b*c) }'`
					PERFMIN=`awk -v a=$tbmin -v b=$NSINTERVAL 'BEGIN { printf("%.5f\n", a/b) }'`;
					PERFMAX=`awk -v a=$tbmax -v b=$NSINTERVAL 'BEGIN { printf("%.5f\n", a/b) }'`;
					writeperf

					let TOT_BYTES=TOT_BYTES+tbtot
				fi
				l_name=$NAME

				iraw=
				imin=
				imax=
				itot=

				oraw=
				omin=
				omax=
				otot=

				traw=
				tmin=
				tmax=
				ttot=

				ibraw=
				ibmin=
				ibmax=
				ibtot=

				obraw=
				obmin=
				obmax=
				obtot=

				tbraw=
				tbmin=
				tbmax=
				tbtot=
			fi

			# Update packets
			if [ -n "$iraw" ]; then
				let idelta=IPKTS-iraw
				if [ -z "$itot" ]; then
					itot=$idelta; imin=$idelta; imax=$idelta;
				else
					let itot=itot+idelta
					if [ "$idelta" -lt "$imin" ] ; then
						imin=$idelta
					fi
					if [ "$idelta" -gt "$imax" ] ; then
						imax=$idelta
					fi
				fi
			fi

			if [ -n "$oraw" ]; then
				let odelta=OPKTS-oraw
				if [ -z "$otot" ]; then
					otot=$odelta; omin=$odelta; omax=$odelta;
				else
					let otot=otot+odelta
					if [ "$odelta" -lt "$omin" ] ; then
						omin=$odelta
					fi
					if [ "$odelta" -gt "$omax" ] ; then
						omax=$odelta
					fi
				fi
			fi

			if [ -n "$traw" ]; then
				let tdelta=IPKTS+OPKTS-traw
				if [ -z "$ttot" ]; then
					ttot=$tdelta; tmin=$tdelta; tmax=$tdelta;
				else
					let ttot=ttot+tdelta
					if [ "$tdelta" -lt "$tmin" ] ; then
						tmin=$tdelta
					fi
					if [ "$tdelta" -gt "$tmax" ] ; then
						tmax=$tdelta
					fi
				fi
			fi
					
			if [ -n "$ibraw" ]; then
				let ibdelta=IBYTES-ibraw
				if [ -z "$ibtot" ]; then
					ibtot=$ibdelta; ibmin=$ibdelta; ibmax=$ibdelta;
				else
					let ibtot=ibtot+ibdelta
					if [ "$ibdelta" -lt "$ibmin" ] ; then
						ibmin=$ibdelta
					fi
					if [ "$ibdelta" -gt "$ibmax" ] ; then
						ibmax=$ibdelta
					fi
				fi
			fi

			if [ -n "$obraw" ]; then
				let obdelta=OBYTES-obraw
				if [ -z "$obtot" ]; then
					obtot=$obdelta; obmin=$obdelta; obmax=$obdelta;
				else
					let obtot=obtot+obdelta
					if [ "$obdelta" -lt "$obmin" ] ; then
						obmin=$obdelta
					fi
					if [ "$obdelta" -gt "$obmax" ] ; then
						obmax=$obdelta
					fi
				fi
			fi
			if [ -n "$tbraw" ]; then
				let tbdelta=IBYTES+OBYTES-tbraw
				if [ -z "$tbtot" ]; then
					tbtot=$tbdelta; tbmin=$tbdelta; tbmax=$tbdelta;
				else
					let tbtot=tbtot+tbdelta
					if [ "$tbdelta" -lt "$tbmin" ] ; then
						tbmin=$tbdelta
					fi
					if [ "$tbdelta" -gt "$tbmax" ] ; then
						tbmax=$tbdelta
					fi
				fi
			fi

			iraw=$IPKTS
			oraw=$OPKTS
			let traw=IPKTS+OPKTS			
			ibraw=$IBYTES
			obraw=$OBYTES
			let tbraw=IBYTES+OBYTES			
		done

		# The min and max values for the following counter are not calculated!!!
		PERFCLASS='Server';PERFINSTANCE='_Total';PERFMETRIC='Bytes Total/sec';PERFINT=$NSINTERVAL;PERFSAMP=$NSSAMPLES
		PERFAVG=`awk -v a=$TOT_BYTES -v b=$NSSAMPLES -v c=$NSINTERVAL 'BEGIN { print a/(b*c) }'`
		PERFMIN=$PERFAVG
		PERFMAX=$PERFAVG
		writeperf
  	)
}

aix_netstat()
{
  cmd="ifconfig"
  test_tools
  [ "$check" -eq 0 ] && return
  cmd="netstat"
  test_tools
  [ "$check" -eq 0 ] && return

  NSINTERVAL=$PerfDuration
  NSSAMPLES=$PerfNumSamples
  TOT_BYTES_PER_SEC=0

  ifconfig -l | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) { print $i;} }' | 
  ( 
    while read NAME ; do
      [ -z "$NAME" ] && continue

      i=0
      start=1
      TEMP=""
      while : ; do
        NETSTAT=`netstat -v "$NAME" 2>&1`
        TEMP=`echo "$NETSTAT" | grep '^Packets:' | tr -d "Packets:"`
        [ -z "$TEMP" ] && break
        TOTOPKTS=`echo "$TEMP" | awk '{print $1}'`
        TOTIPKTS=`echo "$TEMP" | awk '{print $2}'`
        TEMP=`echo "$NETSTAT" | grep '^Bytes:' | tr -d "Bytes:"`
        [ -z "$TEMP" ] && break
        OBYTES=`echo "$TEMP" | awk '{print $1}'`
        IBYTES=`echo "$TEMP" | awk '{print $2}'`

        if [ "$start" -eq 1 ] ; then
          # This is the first line of info for this interface
          IPKTS=$TOTIPKTS
          imin=$IPKTS
          imax=0
          itot=0

          OPKTS=$TOTOPKTS
          omin=$OPKTS
          omax=0
          otot=0

          bytes=`expr $IBYTES + $OBYTES`
          bytesmin=$bytes
          bytesmax=0
          bytestot=0

          start=0
        else
          ipkts_per_sec=`expr $TOTIPKTS - $IPKTS`
          IPKTS=`expr $IPKTS + $ipkts_per_sec`
          opkts_per_sec=`expr $TOTOPKTS - $OPKTS`
          OPKTS=`expr $OPKTS + $opkts_per_sec`
          bytes_per_sec=`expr $IBYTES + $OBYTES - $bytes`
          bytes=`expr $bytes + $bytes_per_sec`

          # Update totals
          itot=`echo $itot + $ipkts_per_sec | bc -l`
          otot=`echo $otot + $opkts_per_sec | bc -l`
          bytestot=`echo $bytestot + $bytes_per_sec | bc -l`

          # Update min value
          if [ "$ipkts_per_sec" -lt "$imin" ] ; then
            imin=$ipkts_per_sec
          fi
          if [ "$opkts_per_sec" -lt "$omin" ] ; then
            omin=$opkts_per_sec
          fi
          if [ "$bytes_per_sec" -lt "$bytesmin" ] ; then
            bytesmin=$bytes_per_sec
          fi
          # Update max value
          if [ "$ipkts_per_sec" -gt "$imax" ] ; then
            imax=$ipkts_per_sec
          fi
          if [ "$opkts_per_sec" -gt "$omax" ] ; then
            omax=$opkts_per_sec
          fi
          if [ "$bytes_per_sec" -gt "$bytesmax" ] ; then
            bytesmax=$bytes_per_sec
          fi
        fi
        i=`expr $i + 1`
        if [ $i -gt $NSSAMPLES ] ; then
          # Class,Inst,Metric,PerfNumSamples,interval,min,max,avg
          PERFCLASS='Network Interface';PERFINSTANCE=$NAME;PERFMETRIC='Packets Received/sec';PERFINT=$NSINTERVAL;PERFSAMP=$NSSAMPLES;PERFAVG=`echo $itot / \( $NSSAMPLES \* $NSINTERVAL \) | bc -l`;PERFMIN=`echo $imin / $NSINTERVAL | bc -l`;PERFMAX=`echo $imax / $NSINTERVAL | bc -l`;writeperf
          PERFCLASS='Network Interface';PERFINSTANCE=$NAME;PERFMETRIC='Packets Sent/sec';PERFINT=$NSINTERVAL;PERFSAMP=$NSSAMPLES;PERFAVG=`echo $otot / \( $NSSAMPLES \* $NSINTERVAL \) | bc -l`;PERFMIN=`echo $omin / $NSINTERVAL | bc -l`;PERFMAX=`echo $omax / $NSINTERVAL | bc -l`;writeperf
          PERFCLASS='Network Interface';PERFINSTANCE=$NAME;PERFMETRIC='Bytes Total/sec';PERFINT=$NSINTERVAL;PERFSAMP=$NSSAMPLES;PERFAVG=`echo $bytestot / \( $NSSAMPLES \* $NSINTERVAL \) | bc -l`;PERFMIN=$bytesmin;PERFMAX=$bytesmax;writeperf
          TOT_BYTES_PER_SEC=`expr $TOT_BYTES_PER_SEC + $bytestot`
          break
        fi
        sleep $NSINTERVAL
      done
    done

    # The min and max values for the following counter are not calculated!!!
    PERFCLASS='Server';PERFINSTANCE='_Total';PERFMETRIC='Bytes Total/sec';PERFINT=$NSINTERVAL;PERFSAMP=$NSSAMPLES;PERFAVG=`echo $TOT_BYTES_PER_SEC / \( $NSSAMPLES \* $NSINTERVAL \) | bc -l`;PERFMIN=$PERFAVG;PERFMAX=$PERFAVG;writeperf
  )
}

do_proc()
{
  SZ=sz
  [ "$OS_TYPE" = "SunOS" ] && SZ=osz
  UNIX95=1
  export UNIX95

  cmd="ps"
  test_tools
  [ "$check" -eq 0 ] && return

  ps -e -o uid,pid,$SZ,vsz,time,fname,pcpu,pmem |
  ( # Discard the header
	read LINE

	# Process the data
	while read SERUID PID SZ VSZ TIME CMD PCPU PMEM IGNORED ; do
		# skip defunct processes
		if [ \( -n "$PMEM" \) -a \( "$PMEM" != "-" \) ] ; then
			CMDNAME=$CMD

			PERFCLASS=Process;PERFINSTANCE=$CMDNAME;PERFMETRIC='Working Set';PERFINT=0;PERFSAMP=1;PERFAVG=$SZ;PERFMIN=$SZ;PERFMAX=$SZ;writeperf
			PERFCLASS=Process;PERFINSTANCE=$CMDNAME;PERFMETRIC='Virtual Bytes';PERFINT=0;PERFSAMP=1;PERFAVG=$VSZ;PERFMIN=$VSZ;PERFMAX=$VSZ;writeperf
			PERFCLASS=Process;PERFINSTANCE=$CMDNAME;PERFMETRIC='% Processor Time';PERFINT=0;PERFSAMP=1;PERFAVG=$PCPU;PERFMIN=$PCPU;PERFMAX=$PCPU;writeperf
			PERFCLASS=Process;PERFINSTANCE=$CMDNAME;PERFMETRIC='% Memory Utilization';PERFINT=0;PERFSAMP=1;PERFAVG=$PMEM;PERFMIN=$PMEM;PERFMAX=$PMEM;writeperf
			# PERFCLASS=Process;PERFINSTANCE=$CMDNAME;PERFMETRIC=UserId;PERFINT=0;PERFSAMP=1;PERFAVG=$SERUID;PERFMIN=$SERUID;PERFMAX=$SERUID;writeperf
			# PERFCLASS=Process;PERFINSTANCE=$CMDNAME;PERFMETRIC=Time;PERFINT=0;PERFSAMP=1;PERFAVG=$TIME;PERFMIN=$TIME;PERFMAX=$TIME;writeperf
			# PERFCLASS=Process;PERFINSTANCE=$CMDNAME;PERFMETRIC=CmdOnly;PERFINT=0;PERFSAMP=1;PERFAVG=$CMD;PERFMIN=$CMD;PERFMAX=$CMD;writeperf
		fi
		PMEM=
	done
	)
}

aix_proc()
{
  cmd="ps"
  test_tools
  [ "$check" -eq 0 ] && return

  ps -e -o vsz,comm,pcpu,pmem |
  (
    # Discard the header
    read LINE

    # Process the data
    while read VSZ CMD PCPU PMEM IGNORED ; do
      [ -z "$CMD" ] && continue
      PERFCLASS=Process;PERFINSTANCE=$CMD;PERFMETRIC='Virtual Bytes';PERFINT=0;PERFSAMP=1;PERFAVG=$VSZ;PERFMIN=$VSZ;PERFMAX=$VSZ;writeperf
      PERFCLASS=Process;PERFINSTANCE=$CMD;PERFMETRIC='% Processor Time';PERFINT=0;PERFSAMP=1;PERFAVG=$PCPU;PERFMIN=$PCPU;PERFMAX=$PCPU;writeperf
      PERFCLASS=Process;PERFINSTANCE=$CMD;PERFMETRIC='% Memory Utilization';PERFINT=0;PERFSAMP=1;PERFAVG=$PMEM;PERFMIN=$PMEM;PERFMAX=$PMEM;writeperf
      CMD=
    done
  )
}

# **************************************************************************
#  Main Routine
# **************************************************************************
PATH=/bin:/usr/bin:/usr/sbin:/sbin:/usr/contrib/bin:/usr/ucb
export PATH

LANG=C
export LANG
umask 022

# Directory to hold temporary files
MYTMPDIR=${MRMPDATADIR:-/tmp}
if [ ! -d "$MYTMPDIR" ] ; then
  exit
fi

# Default settings
PerfDuration=4
PerfNumSamples=3

# Initial setup
CONFIGFILE=$MRMPDIR/mrmpconfigperf.sh
if [ ! -r "$CONFIGFILE" ] ; then
	CONFIGFILE=./mrmpconfigperf.sh
fi
if [ ! -r "$CONFIGFILE" ] ; then
	CONFIGFILE=../mrmpconfigperf.sh
fi
if [ ! -r "$CONFIGFILE" ] ; then
	CONFIGFILE=/tmp/mrmpconfigperf.sh
fi
if [ -r "$CONFIGFILE" ]; then
	. $CONFIGFILE
fi

do_control

case "$OS_TYPE" in
   HP-UX)
      hpux_vmstat
      hpux_iostat
      hpux_get_disk_transfers
      hpuxsun_netstat
      hpux_get_network_transfers
      do_fs
      do_sys
      hpux_swap
      ;;

   Linux)
      linux_vmstat
      linux_iostat
      linux_netstat
      do_fs
      do_sys
      linux_swap
      ;;

   SunOS)
      sun_vmstat
      sun_iostat
      hpuxsun_netstat
      sun_get_network_transfers
      do_fs
      do_sys
      sun_swap
      ;;

   AIX)
      aix_vmstat
      aix_get_cache 
      aix_iostat
      aix_netstat
      do_fs
      do_sys
      aix_swap
      ;;
esac
exit 0
