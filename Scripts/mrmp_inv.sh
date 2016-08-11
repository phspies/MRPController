#!/bin/bash
#  (c) 2016 Dimension Data, All rights reserved.
# **************************************************************************
#  @(#)Module:  UNIX MRMP Inventory Script
#  @(#)Desc:    This script runs all the UNIX/Linux utilities to collect inventory
#  @(#)Version: 1.0.0.0
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
	echo FILETYPE=Log
	echo DATE=`date -uR`
	echo TIMESTAMP=`date '+%Y%m%d%H%M%S'`
	echo CMD=$CMD
	echo ERR=$VAL
	echo '</TOOLS>'
	echo ""''
}

test_tools()
{
	 check=0
	 tmpout=$MYTMPDIR/tmpout.$$
	 tmperr=$MYTMPDIR/tmperr.$$
	 TMP=`which $cmd 2>/dev/null`
	 if [ -x "$TMP" ] ; then
	     if [ -z "$NO_EXEC" ] ; then
			 IMPL=`$cmd 1>$tmpout 2>$tmperr`
	         FOUND=`grep "not implemented" $tmperr`
	         FOUND=`grep "not registered" $tmperr`
	         FOUND=`grep -i "Permission denied" $tmperr`
	     fi
		 if [ ! -z "$FOUND" ] && [ ! "$cmd" = "sudo" ] && [ "$sudo_check" = 1 ]; then
			##no sense in trying to do sudo sudo
			IMPL=`echo |sudo -S $cmd 1>$tmpout 2>$tmperr`
	        FOUND=`grep "not implemented" $tmperr`
	        FOUND=`grep "not registered" $tmperr`
	        FOUND=`grep -i "Permission denied" $tmperr`
			FOUND=`grep -i "password" $tmperr`
			if [ -z "$FOUND" ] ; then
				sudocommands="${sudocommands}${cmd}:" 
			fi
	     fi
	 fi
     	 if [ -x "$TMP" ] && [ -z "$FOUND" ] ; then
	     check=1
	 elif [ -n "$LOG_ERR" ] ; then
	     CMD="$cmd"
	     if [ -s $tmperr ] ; then
             VAL=`cat $tmperr`
	     else
             VAL="not found in $PATH"
	     fi
	     log
	 fi
	 `rm -f $tmperr`
	 `rm -f $tmpout`
}

do_vm_detection()
{
	# Only Linux and Solaris OS is supported as VM
	case "$OS_TYPE" in
		Linux)
			if [ ! -e /proc/scsi/scsi ] || [ -r /proc/vmware/version ] ; then
				echo ISRV_VM=Physical
			else
				LOG_ERR=
				cmd="sed"
				test_tools
				LOG_ERR=1
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
	LOG_ERR=1

	echo '<CONTROL>'
	echo VERSION=100
	if [ "$OS_TYPE" = "HP-UX" ] ; then
 		echo HOSTNAME=`hostname`
	else
		echo HOSTNAME=`uname -n`
	fi
	echo FILETYPE=Inv
	echo DATE=`date -uR`
	echo TIMESTAMP=`date '+%Y%m%d%H%M%S'`
	echo NISDOMAIN=`domainname 2>/dev/null`
	echo UNAME=`uname -a`
	echo RUNBY=`id`
	echo ISRV_Type=2048
	do_vm_detection
	echo '</CONTROL>'
	echo ""
}

test_general_tools()
{
	cmd="awk"
	test_tools
	[ "$check" -eq 0 ] && exit 0
	cmd="sed"
	test_tools
	[ "$check" -eq 0 ] && exit 0
}

do_test_default()
{
	cmd="sudo"
	test_tools
	sudo_check=$check
	cmd="showmount"
	test_tools
	showmount_check=$check
	cmd="dmesg"
	test_tools
	dmesg_check=$check
	if [ "$OS_TYPE" = "HP-UX" ] ; then
		cmd="bdf"
	else
		cmd="df"
	fi
	test_tools
	df_check=$check
}

do_test_aix()
{
	cmd="lslpp"
	test_tools
	lslpp_check=$check
	cmd="lsattr"
	test_tools
	lsattr_check=$check
	cmd="lsdev"
	test_tools
	lsdev_check=$check
	cmd="lsparent"
	test_tools
	lsparent_check=$check
	cmd="lscfg"
	test_tools
	lscfg_check=$check
	cmd="ifconfig"
	test_tools
	ifconfig_check=$check
	cmd="entstat"
	test_tools
	entstat_check=$check
	cmd="bootinfo"
	test_tools
	bootinfo_check=$check
	cmd="getconf"
	test_tools
	getconf_check=$check
	cmd="file"
	test_tools
	file_check=$check
}

do_test_hpux()
{
      cmd="swlist"
      test_tools
	swlist_check=$check
	cmd="ioscan"
	test_tools
	ioscan_check=$check
	cmd="model"
	test_tools
	model_check=$check
	cmd="lanscan"
	test_tools
	lanscan_check=$check
	cmd="machinfo"
	test_tools
	machinfo_check=$check
	cmd="getconf"
	test_tools
	getconf_check=$check
	cmd="netstat"
	test_tools
	netstat_check=$check
	NO_EXEC=1
	cmd="cstm"
	test_tools
	cstm_check=$check
	cmd="adb"
	test_tools
	adb_check=$check
	cmd="lanadmin"
	test_tools
	lanadmin_check=$check
}

do_test_sun()
{
      cmd="prtdiag"
      test_tools
	prtdiag_check=$check
	cmd="psrinfo"
	test_tools
	psrinfo_check=$check
	cmd="prtpicl"
	test_tools
	prtpicl_check=$check
	cmd="prtconf"
	test_tools
	prtconf_check=$check
	cmd="pkginfo"
	test_tools
	pkginfo_check=$check
	cmd="basename"
	test_tools
	basename_check=$check
	cmd="iostat"
	test_tools
	iostat_check=$check
	cmd="prtvtoc"
	test_tools
	prtvtoc_check=$check
	cmd="ifconfig"
	test_tools
	ifconfig_check=$check
	cmd="kstat"
	test_tools
	kstat_check=$check
	cmd="isainfo"
	test_tools
	isainfo_check=$check
	NO_EXEC=1
	cmd="ndd"
	test_tools
	ndd_check=$check
}

do_test_linux()
{
    cmd="rpm"
    test_tools
    rpm_check=$check
    cmd="dpkg"
    test_tools
    dpkg_check=$check
    cmd="basename"
    test_tools
    basename_check=$check
    cmd="ifconfig"
    test_tools
    ifconfig_check=$check
    cmd="lspci"
    test_tools
    lspci_check=$check
    cmd="dmidecode"
    test_tools
    dmidecode_check=$check
    cmd="runlevel"
    test_tools
    runlevel_check=$check
    cmd="fdisk"
    test_tools
    fdisk_check=$check
    cmd="hdparm"
    test_tools
    hdparm_check=$check
    cmd="ethtool"
    test_tools
    ethtool_check=$check
    cmd="mii-tool"
    test_tools
    mii_tool_check=$check
    cmd="lsdev"
    test_tools
    lsdev_check=$check
    cmd="route"
    test_tools
    route_check=$check
}

do_test_tools()
{
  do_test_default
  case "$OS_TYPE" in
    HP-UX)
	do_test_hpux
	;;
    Linux)
      do_test_linux
	;;
    SunOS)
      do_test_sun
      ;;
    AIX)
      do_test_aix
      ;;
  esac
}

# **************************************************************************
#  System Information Section
# **************************************************************************
do_sys()
{
	echo '<SYSTEM>'
	if [ "$OS_TYPE" = "HP-UX" ] ; then
		if [ "$getconf_check" -eq 1 ] ; then
			xx=`getconf MACHINE_SERIAL`
		fi
		if [ -z "$xx" ] && [ "$cstm_check" -eq 1 ] ; then
			xx=`echo "selclass qualifier system;info;wait;infolog"|cstm|grep 'System Serial Number...:'|awk '{print $4}' 2>/dev/null`
		fi
		echo ISRV_SerialNumber=$xx
		echo ISRV_SystemPartition=`grep -v ^\# /stand/bootconf | head -1 | awk '{print $2}'`
		echo ISRV_BootDir=/stand/vmunix
		echo ISRV_Identity=`uname -i`
		echo ISRV_HostName=`hostname`
	else
		echo ISRV_HostName=`uname -n`

		if [ "$OS_TYPE" = "AIX" ] ; then
			echo ISRV_SerialNumber=`uname -u`
			TIME=`cat /etc/environment | grep '^TZ'`
			echo ISRV_TimeZone=`expr "$TIME" : '[A-Z][^-+0-9]\{1,\}\([-+]\{0,1\}[0-9:]\{1,\}\)[A-Z]\{0,\}' | awk -F: '{if ($1 > 0) print (($1+($2/60))*(-1)); else print (($1-($2/60))*(-1));}'`
		elif [ "$OS_TYPE" = "SunOS" ] ; then
			UTCTime=`date -u '+%H:%M'`
			UTCTime=`echo "$UTCTime" | awk -F: '{printf("%0.2f", $1+($2/60)+0.005)}'`
			LocTime=`date '+%H:%M'`
			LocTime=`echo "$LocTime" | awk -F: '{printf("%0.2f", $1+($2/60)+0.005)}'`
			UTCDate=`date -u '+%y%m%d'`
			LocDate=`date '+%y%m%d'`
			if [ "$LocDate" -gt "$UTCDate" ] ; then
				echo ISRV_TimeZone=`echo "$LocTime + 24 - $UTCTime" | bc -l`
			elif [ "$LocDate" -lt "$UTCDate" ] ; then
				echo ISRV_TimeZone=`echo "$LocTime - 24 - $UTCTime" | bc -l`
			else
				echo ISRV_TimeZone=`echo "$LocTime - $UTCTime" | bc -l`
			fi
		elif [ "$OS_TYPE" = "Linux" ] ; then
			if [ "$dmidecode_check" -eq 1 ] ; then
				if echo $sudocommands | grep -q ":dmidecode:" 
				then
					ISRV_AssetTag=`echo | sudo -S dmidecode 2>/dev/null | grep -A 10 'System Information' | grep 'Asset Tag:'`
					echo ISRV_SerialNum=`echo | sudo -S dmidecode 2>/dev/null| grep -A 10 'System Information' | grep 'Serial Number:' | awk -F": " '{print $2}'`
				else
					ISRV_AssetTag=`dmidecode | grep -A 10 'System Information' | grep 'Asset Tag:'`
					echo ISRV_SerialNum=`dmidecode | grep -A 10 'System Information' | grep 'Serial Number:' | awk -F": " '{print $2}'`
				fi
				if [ -n "$ISRV_AssetTag" ] ; then
					echo ISRV_AssetTag=$ISRV_AssetTag
				fi
			fi
			echo ISRV_TimeZone=`date -R | awk '{print substr($NF, 1, 1)(int(substr($NF, 2, 2)) + (int(substr($NF, 4, 2)) / 60))}'`
		fi
	fi

	echo ISRV_DisplayName=`uname -n`
	echo ISRV_ComputerName=`uname -n`
	echo ISRV_ActiveName=`uname -n`
		  
	echo ISRV_InstallLocale=1033
	echo ISRV_CurrentLocale=1033

	echo ISRV_LastUpdateStatus=Success
	echo ISRV_Path=$PATH
	echo ISRV_Type=2048
	
	echo ISRV_DateTime=`date`
	echo '</SYSTEM>'
	echo ""

	if [ -r /etc/resolv.conf ] ; then
		CGNAME=`grep '^domain' /etc/resolv.conf | awk '{print $2}'`
		if [ -n "$CGNAME" ] ; then
			echo '<GROUP>'
			echo CG_Name=$CGNAME
			echo CGT_Name='Domain Name Service (DNS)'
			echo CGM_MemberType=1
			echo '</GROUP>'
			echo ""
		fi
	fi
}

# **************************************************************************
#  System Operating System Section
# **************************************************************************
do_os()
{
	echo '<OS>'
	echo ISA_Identity=`uname -a`
	echo DAPP_Type=OS
	if [ "$OS_TYPE" = "Linux" ] ; then
		if [ -r /etc/SuSE-release ] ; then
			echo DAPP_Producer=SuSE Linux AG, Nuernberg, Germany
			echo DAPP_Name=`cat /etc/SuSE-release | head -1`
			echo DAPP_DisplayVersion=`cat /etc/SuSE-release | tail -1 | awk '{print $3}'`
			echo DAPP_MajorVersion=`uname -r | awk -F. '{print $1}'`
			echo DAPP_MinorVersion=`uname -r | awk -F. '{print $2}'`
			echo DAPP_PatchLevel=`uname -r | awk -F. '{print $3}'`
                elif [ -r /etc/lsb-release ] ; then
                        echo DAPP_Producer=`lsb_release -d | awk '{print $2, $3, $4, $5}'`
                        DAPP=`cat /etc/issue | head -1`
                        echo DAPP_Name=`lsb_release -i | awk '{print $3}'`
                        echo DAPP_DisplayVersion=`lsb_release -r | awk '{print $2}'`
                        echo DAPP_MajorVersion=`uname -r | awk -F. '{print $1}'`
                        echo DAPP_MinorVersion=`uname -r | awk -F. '{print $2}'`
                        echo DAPP_PatchLevel=`uname -r | awk -F. '{print $3}'`
		elif [ -r /proc/vmware/version ] ; then
			echo DAPP_Producer=VMWare, Inc.
			DAPP=`cat /proc/vmware/version | head -1`
			echo DAPP_Name=`expr "$DAPP" : "\(.*\) [0-9]*\.[0-9]*\..*"`
			echo DAPP_DisplayVersion=`expr "$DAPP" : ".* \([0-9]*\.[0-9]*\.[^\ ]*\) .*"`
			echo DAPP_MajorVersion=`uname -r | awk -F. '{print $1}'`
			echo DAPP_MinorVersion=`uname -r | awk -F. '{print $2}'`
			echo DAPP_PatchLevel=`uname -r | awk -F. '{print $3}'`
		elif [ -r /etc/redhat-release ] ; then
			echo DAPP_Producer=Red Hat
			if grep -q "Red Hat" /etc/redhat-release ; then
				echo DAPP_Name=Red Hat `sed "s/Red Hat \(.*\) release/\1/" /etc/redhat-release`
			else
				echo DAPP_Name=`sed "s/release//g" /etc/redhat-release`
			fi
			echo DAPP_MajorVersion=`uname -r | awk -F. '{print $1}'`
			echo DAPP_MinorVersion=`uname -r | awk -F. '{print $2}'`
			echo DAPP_PatchLevel=`uname -r | awk -F. '{print $3}'`
		else
			echo DAPP_Producer=$OS_TYPE
			echo DAPP_Name=$OS_TYPE
			echo DAPP_DisplayVersion=`uname -r`
			echo DAPP_MajorVersion=`uname -r | awk -F. '{print $1}'`
			echo DAPP_MinorVersion=`uname -r | awk -F. '{print $2}'`
			echo DAPP_PatchLevel=`uname -r | awk -F. '{print $3}'`
		fi
		DAPP_Architecture=`uname -a`
		DAPP_Architecture=`expr "$DAPP_Architecture" : '.\{0,\}[^0-9]\{0,\}\(64\) \{1,\}.\{0,\}'`
	elif [ "$OS_TYPE" = "HP-UX" ] ; then
		echo ISA_InstallDate=`ls -l /stand/vmunix | awk '{print $6,$7,$8}'`
		echo ISA_InstallLoc=/stand/vmunix
		echo ISA_EstimatedSize=`ls -l /stand/vmunix | awk '{print $5}'`
		echo DAPP_Producer=Hewlett-Packard Co.
		echo DAPP_Name=$OS_TYPE
		echo DAPP_DisplayVersion=`uname -r`
		echo DAPP_MajorVersion=`uname -r | awk -F. '{print $1}'`
		echo DAPP_MinorVersion=`uname -r | awk -F. '{print $2}'`
		echo DAPP_PatchLevel=`uname -r | awk -F. '{print $3}'`
		if [ "$getconf_check" -eq 1 ] ; then
			DAPP_Architecture=`getconf KERNEL_BITS`
		fi
		if [ -n "$DAPP_Architecture" ] ; then
			echo DAPP_Architecture="$DAPP_Architecture"
		else
			echo DAPP_Architecture=32
		fi
	elif [ "$OS_TYPE" = "AIX" ] ; then
		echo DAPP_Producer=$OS_TYPE
		echo DAPP_Name=$OS_TYPE
		echo DAPP_DisplayVersion=`oslevel`
		echo DAPP_MajorVersion=`uname -v`
		echo DAPP_MinorVersion=`uname -r`
		echo DAPP_PatchLevel=`oslevel | cut -f3 -d.`
		echo DAPP_OSVersion=`oslevel`
		
		if [ "$getconf_check" -ne 0 ] ; then
			DAPP_Architecture=`getconf KERNEL_BITMODE`
		fi
		if [ -z "$DAPP_Architecture" ] ; then
			if [ "$file_check" -ne 0 ]; then
				set +f
				DAPP_Architecture=`file /usr/lib/boot/unix*`
				set -f
				DAPP_Architecture=`expr "$DAPP_Architecture" : '.\{0,\}[^0-9]\{0,\}\(64\): \{1,\}.\{0,\}'`
				if [ -n "$DAPP_Architecture" ] ; then
					DAPP_Architecture=64
				else
					DAPP_Architecture=32
				fi
				echo DAPP_Architecture="$DAPP_Architecture"
			fi
		fi
	else
		echo DAPP_Producer=$OS_TYPE
		echo DAPP_Name=$OS_TYPE
		echo DAPP_DisplayVersion=`uname -r`
		echo DAPP_MajorVersion=`uname -r | awk -F. '{print $1}'`
		echo DAPP_MinorVersion=`uname -r | awk -F. '{print $2}'`
		echo DAPP_PatchLevel=`uname -r | awk -F. '{print $3}'`
		if [ "$isainfo_check" -ne 0 ] ; then
			DAPP_Architecture=`isainfo -v`
			DAPP_Architecture=`expr "$DAPP_Architecture" : '.\{0,\}[^0-9]\{0,\}\(64\)-bit \{1,\}.\{0,\}'`
		fi
	fi
	if [ "$OS_TYPE" != "HP-UX" ] && [ "$OS_TYPE" != "AIX" ] ; then
		if [ -n "$DAPP_Architecture" ] ; then
			echo DAPP_Architecture=64
		else
			echo DAPP_Architecture=32
		fi
	fi
	echo '</OS>'
	echo ""
}

# **************************************************************************
#  Application Information Section
# **************************************************************************
do_apps_hpux()
{
	[ "$swlist_check" -eq 0 ] && return
	WRITINGPROP=
	swlist -x verbose=1 -l product -a control_directory -a revision -a size -a install_date -a category_tag -a vendor_tag -a location -a install_source | grep -v ^# |
	while read LINE; do
		PARAM=${LINE%% *}
		VALUE=${LINE#* }
		VALUE=`echo $VALUE | sed 's/^ *//'`

		if [ "$PARAM" = "control_directory" ] ; then
			 echo '<APP>'
			 WRITINGPROP=1
			 echo ISA_Identity=$VALUE
			 echo ISA_ProductID=$VALUE
			 echo DAPP_Name=$VALUE
			 echo DAPP_Type=App
		elif [ "$LINE" = "" ] ; then
			 if [ -n "$WRITINGPROP" ] ; then
				WRITINGPROP=
				echo '</APP>'
				echo ""
			 fi
		elif [ "$PARAM" = "" ] ; then
			 echo DAPP_Name=$VALUE
		elif [ "$PARAM" = "category_tag" ] ; then
			 echo DAPP_SubType=$VALUE
		elif [ "$PARAM" = "revision" ] ; then
			 echo DAPP_DisplayVersion=$VALUE
			 echo DAPP_MajorVersion=`echo "$VALUE" | awk -F. '{print $1}'`
			 echo DAPP_MinorVersion=`echo "$VALUE" | awk -F. '{print $2}'`
			 echo DAPP_PatchLevel=`echo "$VALUE" | awk -F. '{print $3}'`
		elif [ "$PARAM" = "vendor_tag" ] ; then
			 echo DAPP_Producer=$VALUE
		elif [ "$PARAM" = "title" ] ; then
			 echo DAPP_Description=$VALUE
		elif [ "$PARAM" = "location" ] ; then
			 echo ISA_InstallLoc=$VALUE
		elif [ "$PARAM" = "install_source" ] ; then
			 echo ISA_InstallSource=$VALUE
		elif [ "$PARAM" = "install_date" ] ; then
			 echo ISA_InstallDate=$VALUE
		elif [ "$PARAM" = "size" ] ; then
			 echo ISA_EstimatedSize=$VALUE
		fi
	done
	if [ "$WRITINGPROP" = "1" ] ; then
		echo '</APP>'
		echo ""
	fi
}

do_apps_linux()
{
	[ "$rpm_check" -eq 0 ] && return
	rpm -q -a --qf '%{VENDOR}*%{NAME}*%{VERSION}.%{RELEASE}*%{SIZE}*%{DIRNAMES}*%{SUMMARY}*%{INSTALLTIME:date}*%{SOURCERPM}*%{LICENSE}*%{DISTRIBUTION}\n' |
	while read LINE ; do
		echo '<APP>'
		ID=`echo "$LINE" | awk -F* '{print $2}'`
		echo DAPP_Type=App
		echo ISA_Identity=$ID
		echo ISA_ProductID=$ID
		echo ISA_EstimatedSize=`echo "$LINE" | awk -F* '{print $4}'`
		echo ISA_InstallLoc=`echo "$LINE" | awk -F* '{print $5}'`
		echo ISA_InstallDate=`echo "$LINE" | awk -F* '{print $7}'`
		echo ISA_InstallSource=`echo "$LINE" | awk -F* '{print $8}'`
		echo ISA_RegOwner=`echo "$LINE" | awk -F* '{print $9}'`
		echo ISA_RegCompany=`echo "$LINE" | awk -F* '{print $10}'`
		echo DAPP_Producer=`echo "$LINE" | awk -F* '{print $1}'`
		echo DAPP_Name=`echo "$LINE" | awk -F* '{print $2}'`

		VERS=`echo "$LINE" | awk -F* '{print $3}'`
		echo DAPP_DisplayVersion=$VERS
		echo DAPP_MajorVersion=`echo "$VERS" | awk -F. '{print $1}'`
		echo DAPP_MinorVersion=`echo "$VERS" | awk -F. '{print $2}'`
		echo DAPP_PatchLevel=`echo "$VERS" | awk -F. '{print $3}'`
		echo DAPP_Description=`echo "$LINE" | awk -F* '{print $6}'`
		echo '</APP>'
		echo ""
	done

        [ "$dpkg_check" -eq 0 ] && return
        dpkg-query -W -f='${Package}*${VERSION}*${Size}*${DIRNAMES}*${SUMMARY}*${INSTALLTIME:date}*${LICENSE}*${Architecture}\n'
        while read LINE ; do
                echo '<APP>'
                ID=`echo "$LINE" | awk -F* '{print $2}'`
                echo DAPP_Type=App
                echo ISA_Identity=$ID
                echo ISA_ProductID=$ID
                echo ISA_EstimatedSize=`echo "$LINE" | awk -F* '{print $4}'`
                echo ISA_InstallLoc=`echo "$LINE" | awk -F* '{print $5}'`
                echo ISA_InstallDate=`echo "$LINE" | awk -F* '{print $7}'`
                echo ISA_InstallSource=`echo "$LINE" | awk -F* '{print $8}'`
                echo ISA_RegOwner=`echo "$LINE" | awk -F* '{print $9}'`
                echo ISA_RegCompany=`echo "$LINE" | awk -F* '{print $10}'`
                echo DAPP_Producer=`echo "$LINE" | awk -F* '{print $1}'`
                echo DAPP_Name=`echo "$LINE" | awk -F* '{print $2}'`

                VERS=`echo "$LINE" | awk -F* '{print $3}'`
                echo DAPP_DisplayVersion=$VERS
                echo DAPP_MajorVersion=`echo "$VERS" | awk -F. '{print $1}'`
                echo DAPP_MinorVersion=`echo "$VERS" | awk -F. '{print $2}'`
                echo DAPP_PatchLevel=`echo "$VERS" | awk -F. '{print $3}'`
                echo DAPP_Description=`echo "$LINE" | awk -F* '{print $6}'`
                echo '</APP>'
                echo ""
        done

}

do_apps_sun()
{
	[ "$pkginfo_check" -eq 0 ] && return
	TMP_APPINFO=./appinfo.$$
	pkginfo -l > $TMP_APPINFO 2>/dev/null
	WRITINGPROP=
	while read LINE; do
		PARAM=`echo $LINE | awk -F: '{print $1}' | sed 's/\ //'`
		VALUE=`echo $LINE | awk -F: '{print $2}' | sed 's/^ *//'`

		if [ "$PARAM" = "PKGINST" ] ; then
			if [ "$WRITINGPROP" = "1" ] ; then
				echo '</APP>'
				echo ""
			fi
			echo '<APP>'
			WRITINGPROP=1
			echo ISA_Identity=$VALUE
			echo ISA_ProductID=$VALUE
			echo DAPP_Type=App
		elif [ "$PARAM" = "NAME" ] ; then
			echo DAPP_Name=$VALUE
		elif [ "$PARAM" = "CATEGORY" ] ; then
			echo DAPP_SubType=$VALUE
		elif [ "$PARAM" = "VERSION" ] ; then
			echo DAPP_DisplayVersion=$VALUE
			echo DAPP_MajorVersion=`echo "$VALUE" | awk -F. '{print $1}'`
			echo DAPP_MinorVersion=`echo "$VALUE" | awk -F. '{print $2}'`
			echo DAPP_PatchLevel=`echo "$VALUE" | awk -F. '{print $3}'`
		elif [ "$PARAM" = "BASEDIR" ] ; then
			echo ISA_InstallLoc=$VALUE
		elif [ "$PARAM" = "VENDOR" ] ; then
			echo DAPP_Producer=$VALUE
		elif [ "$PARAM" = "DESC" ] ; then
			echo DAPP_Description=$VALUE
		elif [ "$PARAM" = "INSTDATE" ] ; then
			echo ISA_InstallDate=$VALUE
		fi
	done < $TMP_APPINFO
	if [ -r "$TMP_APPINFO" ] ; then
		echo '</APP>'
		echo ""
		rm -f $TMP_APPINFO
	fi
}

do_apps_aix()
{
	[ "$lslpp_check" -eq 0 ] && return
	PREVPKGNAME=
	lslpp -cqL 2>/dev/null |
	while read LINE; do
		PKGNAME=`echo $LINE | awk -F: '{print $1}'`
		if [ "$PKGNAME" != "$PREVPKGNAME" ] ; then
			PREVPKGNAME=$PKGNAME
			FILESET=`echo $LINE | awk -F: '{print $2}'`
			VERSION=`echo $LINE | awk -F: '{print $3}'`
			STATE=`echo $LINE | awk -F: '{print $4}'`
			PTFID=`echo $LINE | awk -F: '{print $5}'`
			FIXSTATE=`echo $LINE | awk -F: '{print $6}'`
			APPTYPE=`echo $LINE | awk -F: '{print $7}'`
			APPDESC=`echo $LINE | awk -F: '{print $8}'`

			echo '<APP>'
			echo ISA_Identity=$PKGNAME::$VERSION
			echo ISA_ProductID=$PKGNAME
			echo DAPP_Type=App
			echo DAPP_Name=$APPDESC
			echo DAPP_SubType=$VALUE
			echo DAPP_DisplayVersion=$VERSION
			echo DAPP_MajorVersion=`echo "$VERSION" | awk -F. '{print $1}'`
			echo DAPP_MinorVersion=`echo "$VERSION" | awk -F. '{print $2}'`
			DAPP_PatchLevel=`echo "$VERSION" | awk -F. '{print $3}'`
			[ -z "$DAPP_PatchLevel" ] && DAPP_PatchLevel=0
			echo DAPP_PatchLevel="$DAPP_PatchLevel"
			echo DAPP_Description=$APPDESC
			echo '</APP>'
			echo ""
		fi
	done
}

do_apps()
{
	case "$OS_TYPE" in
	  HP-UX)
		 do_apps_hpux
		 ;;
	  Linux)
		 do_apps_linux
		 ;;

	  SunOS)
		 do_apps_sun
		 ;;
	  AIX)
		 do_apps_aix
		 ;;
	esac
}

# **************************************************************************
#  Chassis Information Section
# **************************************************************************
do_chassis()
{
	echo '<MOTHERBOARD>'
	echo DCH_CPUTypeList=$CPU_TYPE
	if [ "$OS_TYPE" = "HP-UX" ] ; then
		if [ "$getconf_check" -eq 1 ] ; then
			echo ISC_SerialNumber=`getconf MACHINE_SERIAL`
			echo ISC_Identity=`getconf MACHINE_IDENT`
			echo DCH_Model=`getconf MACHINE_MODEL`
			echo DCH_CPUTypeList=`getconf CPU_CHIP_TYPE`
		fi
		echo DCH_Make=Hewlett-Packard Co.
		if [ "$ioscan_check" -eq 1 ] ; then
    		    echo DCH_NumBus=`ioscan -fnkC ba | grep ^ba | wc -l`
		fi	
	elif [ "$OS_TYPE" = "Linux" ] ; then
		if [ "$dmidecode_check" -eq 1 ] ; then
			if echo $sudocommands | grep -q ":dmidecode:" 
			then
				cmd_preffix="echo | sudo -S dmidecode"
			else
				cmd_preffix="dmidecode"
			fi
			xx=`eval $cmd_preffix | grep -A 5 'System Information' | head -5  | grep 'Manufacturer'`
			DCH_Make=`expr "$xx" : ".*: * *\(.*\)"`
			xx=`eval $cmd_preffix | grep -A 5 'System Information' | head -5  | grep 'Product Name'`
			DCH_Model=`expr "$xx" : ".*: * *\(.*\)"`
			xx=`eval $cmd_preffix | grep -A 5 'System Information' | head -5  | grep 'Version'`
			DCH_Version=`expr "$xx" : ".*: * *\(.*\)"`
			xx=`eval $cmd_preffix | grep -A 5 'System Information' | head -5  | grep 'Serial Number'`
			ISC_SerialNumber=`expr "$xx" : ".*: * *\(.*\)"`
			if [ -n "$ISC_SerialNumber" ] ; then
				ISC_Identity=$ISC_SerialNumber
			fi

			if [ -z "$DCH_Make" ] ; then
				xx=`eval $cmd_preffix | grep -A 5 'Base Board Information' | head -5 | grep 'Manufacturer'`
				DCH_Make=`expr "$xx" : ".*: * *\(.*\)"`
			fi
			if [ -z "$DCH_Model" ] ; then
				xx=`eval $cmd_preffix | grep -A 5 'Base Board Information' | head -5 | grep 'Product Name'`
				DCH_Model=`expr "$xx" : ".*: * *\(.*\)"`
			fi
			if [ -z "$DCH_Version" ] ; then
				xx=`eval $cmd_preffix | grep -A 5 'Base Board Information' | head -5 | grep 'Version'`
				DCH_Version=`expr "$xx" : ".*: * *\(.*\)"`
			fi
			ISC_AssetTag_dmidecode=`eval $cmd_preffix | grep -A 9 'Chassis Information' | grep 'Asset Tag:' | awk -F": " '{print $2}'`

			xx=`eval $cmd_preffix | grep -A 5 'BIOS Information' | head -5  | grep 'Vendor'`
			DCH_BiosMake=`expr "$xx" : ".*: * *\(.*\)"`
			xx=`eval $cmd_preffix | grep -A 5 'BIOS Information' | head -5  | grep 'Release'`
			ISC_BiosDate=`expr "$xx" : ".*: * *\(.*\)"`
			xx=`eval $cmd_preffix | grep -A 5 'BIOS Information' | head -5  | grep 'Version'`
			ISC_BiosVersion=`expr "$xx" : ".*: * *\(.*\)"`
			xx=`eval $cmd_preffix | grep -A 22 'Memory Controller Information' | head -22 | grep 'Maximum Total Memory Size'`
			DCH_RamMax=`expr "$xx" : ".*: * *\([0-9]*\)"`
			MemUnits=`echo "$xx" | awk '{print $NF}'`
			xx=`eval $cmd_preffix | grep -A 22 'Memory Controller Information' | head -22 | grep 'Associated Memory Slots'`
			DCH_RamNumSlots=`expr "$xx" : ".*: * *\([0-9]*\)"`
			SysMem=`eval $cmd_preffix | grep -A 6 'Physical Memory Array' | grep 'Use:' | awk -F": " '{if ($2 == "System Memory") print NR}'`
			if [ -z "$DCH_RamMax" ] ; then
				MemUnits=`eval $cmd_preffix | grep -A 6 'Physical Memory Array' | grep 'Maximum Capacity:' | awk -v a=$SysMem -F": " '{if (a == NR) print $2}' | awk '{print $2}'`
				DCH_RamMax=`eval $cmd_preffix | grep -A 6 'Physical Memory Array' | grep 'Maximum Capacity:' | awk -v a=$SysMem -F": " '{if (a == NR) print $2}' | awk '{print $1}'`
			fi
			if [ -z "$DCH_RamNumSlots" ] ; then
				DCH_RamNumSlots=`eval $cmd_preffix | grep -A 6 'Physical Memory Array' | grep 'Number Of Devices:' | awk -v a=$SysMem -F": " '{if (a == NR) print $2}'`
			fi
			DCH_BusNumSlots=`eval $cmd_preffix | grep -A 2 'System Slot Information' | grep 'Type:' | grep -v 'ISA' | wc -l | tr -d ' '`
		fi

		if [ -r "$TMPDMESG" ] ; then
			if [ -z "$DCH_Make" ] ; then
				DCH_Make=`grep '^System Vendor:' $TMPDMESG | cut -d ' ' -f 3-`
			fi
			if [ -z "$DCH_Model" ] ; then
				DCH_Model=`grep '^Product Name:' $TMPDMESG | cut -d ' ' -f 3-`
			fi

			if [ -z "$DCH_Make" ] ; then
				ISCMODEL=`grep ^OEM $TMPDMESG`
				if [ -n "$ISCMODEL" ] ; then
					DCH_Make=`echo $ISCMODEL | awk '{print $3}'`
					DCH_Model=`echo $ISCMODEL | awk '{print $6}'`
				fi
			fi
			if [ -z "$DCH_BiosMake" ] ; then
				DCH_BiosMake=`grep '^BIOS Vendor:' $TMPDMESG | cut -d ' ' -f 3-`
			fi
			if [ -z "$ISC_BiosDate" ] ; then
				ISC_BiosDate=`grep '^BIOS Release:' $TMPDMESG | cut -d ' ' -f 3-`
			fi
			if [ -z "$ISC_BiosVersion" ] ; then
				ISC_BiosVersion=`grep '^BIOS Version:' $TMPDMESG | cut -d ' ' -f 3-`
			fi
			if [ -z "$ISC_SerialNumber" ] ; then
				ISC_SerialNumber=`grep '^Serial Number' $TMPDMESG | cut -d ' ' -f 3-`
				ISC_Identity=$ISC_SerialNumber
			fi
			if [ -z "$ISC_AssetTag" ] ; then
				ISC_AssetTag=`grep '^Asset Tag:' $TMPDMESG | cut -d ' ' -f 3-`
			fi
		fi

		if expr "$CPU_TYPE" : "i.*86" >/dev/null ; then
			if [ -z "$DCH_Make" ] ; then
				DCH_Make='AT/AT COMPATIBLE'
			fi
			if [ -z "$DCH_Model" ] ; then
				DCH_Model='AT/AT COMPATIBLE'
			fi
		fi
		if [ -n "$DCH_Make" ] ; then
			echo DCH_Make=$DCH_Make
		fi
		if [ -n "$DCH_Model" ] ; then
			echo DCH_Model=$DCH_Model
		fi
		if [ -n "$DCH_Version" ] ; then
			echo DCH_Version=$DCH_Version
		fi
		if [ -n "$DCH_BiosMake" ] ; then
			echo DCH_BiosMake=$DCH_BiosMake
		fi
		if [ -n "$DCH_RamMax" ] ; then
			if [ "$MemUnits" = "GB" ] ; then
				echo DCH_RamMax=`awk -v a=$DCH_RamMax 'BEGIN {print a * 1024 * 1024}'`
			elif [ "$MemUnits" = "MB" ] ; then
				echo DCH_RamMax=`awk -v a=$DCH_RamMax 'BEGIN {print a * 1024}'`
			else
				echo DCH_RamMax=$DCH_RamMax
			fi
		fi
		if [ -n "$DCH_RamNumSlots" ] ; then
			echo DCH_RamNumSlots=$DCH_RamNumSlots
		fi
		if [ -n "$DCH_BusNumSlots" ] ; then
			echo DCH_BusNumSlots=$DCH_BusNumSlots
		fi
		if [ -n "$ISC_BiosDate" ] ; then
			echo ISC_BiosDate=$ISC_BiosDate
		fi
		if [ -n "$ISC_BiosVersion" ] ; then
			echo ISC_BiosVersion=$ISC_BiosVersion
		fi
		if [ -n "$ISC_SerialNumber" ] ; then
			echo ISC_SerialNumber=$ISC_SerialNumber
		fi
		if [ -n "$ISC_Identity" ] ; then
			echo ISC_Identity=$ISC_Identity
		fi
		if [ -n "$ISC_AssetTag" ] ; then
			echo ISC_AssetTag=$ISC_AssetTag
		elif [ -n "$ISC_AssetTag_dmidecode" ] ; then
			echo ISC_AssetTag=$ISC_AssetTag_dmidecode
		fi
	elif [ "$OS_TYPE" = "SunOS" ] ; then
	    if [ "$prtdiag_check" -eq 1 ] ; then
		ISCMODEL=`prtdiag | grep '^System Configuration:'`
		if [ -n "$ISCMODEL" ] ; then
			echo DCH_Make=`echo $ISCMODEL | awk '{print $3,$4}'`
			echo DCH_Model=`echo $ISCMODEL | cut -d ' ' -f 6-`
			if [ "$psrinfo_check" -eq 1 ] ; then
				MAXCPU=`psrinfo | wc -l`
				echo DCH_MaxCPUs=`echo $MAXCPU`
			fi
		fi
	    fi
		if [ "$prtpicl_check" -eq 1 ] ; then
			ISC_BiosVersion=`prtpicl -v -c flashprom | grep :version | awk '{print $2}'`
			if [ -n "$ISC_BiosVersion" ] ; then
				echo ISC_BiosVersion=$ISC_BiosVersion
			fi
			ISC_BiosDate=`prtpicl -v -c openprom | grep :version | awk '{print $4}'`
			if [ -n "$ISC_BiosDate" ] ; then
				echo ISC_BiosDate=$ISC_BiosDate
			fi
		fi
	elif [ "$OS_TYPE" = "AIX" ] ; then
		echo DCH_Make=IBM
		echo DCH_Model=`uname -M`
		if [ "$lsattr_check" -eq 1 ] ; then
			ser_num=`lsattr -El sys0 -a systemid | awk '{print $2}' | awk -F, '{print $2}'`
			prefix=`echo ${ser_num} | cut -c3-4`
			suffix=`echo ${ser_num} | cut -c5-9`
			serial=${prefix}-${suffix}
			echo ISC_SerialNumber=$serial
			echo ISC_Identity=$serial
		fi
	fi
	echo '</MOTHERBOARD>'
	echo ""
}

# **************************************************************************
#  CPU Information Section
# **************************************************************************
hpux_cpu_machinfo()
{
  TMP_MACH=$MYTMPDIR/machtmp.$$

  machinfo > $TMP_MACH 2>/dev/null

  # Determine the number of CPU's in the machine
  if [ "$getconf_check" -eq 1 ] ; then
    NUM_CPUS=`getconf NUM_CPUS`
    BIT_LENGTH=`getconf HW_CPU_SUPP_BITS`
  fi
  if [ -z $BIT_LENGTH ] ; then
  	BIT_LENGTH=32
  fi

  xx=`< $TMP_MACH grep "Clock speed"`
  CPU_CLOCKSPEED=`echo $xx | awk '{print $4}'`

  if [ -z "$CPU_CLOCKSPEED" ] ; then
    CPU_CLOCKSPEED=`grep 'MHz' $TMP_MACH | head -n 1 | awk -F'(' '{print $2}' | awk -F' MHz' '{print $1}'`
  fi

  xx=`< $TMP_MACH grep "vendor information"`
  CPU_VENDORINFO=`expr "$xx" : ".*= *\(.*\)" | sed s/\"//g`

  xx=`< $TMP_MACH grep "processor serial number"`
  CPU_PROCSERIAL=`echo $xx | awk '{print $5}' | sed s/0x//`

  xx=`< $TMP_MACH grep "processor family"`
  CPU_FAMILY_DESC=`expr "$xx" : ".*: *[0-9]* *\(.*\)"`
  CPU_FAMILY=`echo $xx | awk '{print $3}'`

  xx=`< $TMP_MACH grep "processor model:"`
  CPU_MODEL=`echo $xx | awk '{print $3}'`

  if [ -z "$CPU_MODEL" ] ; then
    CPU_MODEL=`grep 'Model:' $TMP_MACH | awk -F':' '{print $2}' | tr -d '"' | tr -d ' '`
  fi

  xx=`< $TMP_MACH grep "processor revision:"`
  CPU_REV=`echo $xx | awk '{print $3}'`

  if [ -z "$CPU_REV" ] ; then
    CPU_REV=`grep 'CPU version' $TMP_MACH | awk -F'version ' '{print $2}'`
  fi

  # L1 instruction cache
  xx=`< $TMP_MACH grep "L1 Instruction:"`
  CACHE1_I=`echo $xx | awk '{print $5}'`

  # L1 data cache
  xx=`< $TMP_MACH grep "L1 Data:"`
  CACHE1_D=`echo $xx | awk '{print $5}'`

  xx=`< $TMP_MACH grep "L2 Unified:"`
  CACHE2=`echo $xx | awk '{print $5}'`

  xx=`< $TMP_MACH grep "L3 Unified:"`
  CACHE3=`echo $xx | awk '{print $5}'`

  rm -f $TMP_MACH

  # Print the same data for all CPU's
  let i=0;
  while [ $i -eq 0 ] || [ $i -lt "$NUM_CPUS" ] ; do
    echo '<CPU>'
    echo ISC_Identity=$i
    echo ISC_SlotNumber=$i 
    echo ISC_CurCPUSpeed=$CPU_CLOCKSPEED
    echo DCPU_Make=$CPU_VENDORINFO
    echo DCPU_Desc=$CPU_FAMILY_DESC
    # The serial-number is only obtained for the 1st CPU
    if [ -n "$CPU_PROCSERIAL" ] ; then
      echo ISC_SerialNumber=$CPU_PROCSERIAL
      CPU_PROCSERIAL=""
    fi
    echo DCPU_Family=$CPU_FAMILY
    echo DCPU_ModelNum=$CPU_MODEL
    echo DCPU_Stepping=$CPU_REV
    echo DCPU_Model=ia64 Family $CPU_FAMILY Model $CPU_MODEL Stepping $CPU_REV
    echo DCPU_RatedSpeed=$CPU_CLOCKSPEED
    echo DCPU_PrimCacheInst=$CACHE1_I
    echo DCPU_PrimCacheData=$CACHE1_D
    echo DCPU_SecCacheSize=$CACHE2
    echo DCPU_ThirdCacheSize=$CACHE3
    echo DCPU_Architecture=$BIT_LENGTH
    echo '</CPU>'
    echo ""
    let i=i+1
  done
}

hpux_cpu_cstm()
{
  TMP_CSTM=$MYTMPDIR/cstmtmp.$$

  echo "selclass qualifier cpu;info;wait;infolog"|cstm > $TMP_CSTM 2>/dev/null

  # Determine the number of CPU's in the machine
  if [ "$ioscan_check" -eq 1 ] ; then
      NUM_CPUS=`ioscan -fnkC processor | grep processor | wc -l`
  fi

  if [ "$adb_check" -eq 1 ] ; then
     CPU_CLOCKSPEED=`echo "itick_per_usec/D"|adb -k /stand/vmunix /dev/kmem | tail -1 | awk '{print $2}'`
  fi

  if [ "$model_check" -eq 1 ] ; then
    xx=`model|cut -d/ -f2`
    if [ $xx -eq 800 ]; then
       CPU_FAMILY=1.0
    elif [ $xx -eq 700 ]; then
       CPU_FAMILY=1.1
    fi

    xx=`model|cut -d/ -f3`
    CPU_MODEL=`grep ^$xx /usr/sam/lib/mo/sched.models | awk '{print $3}'`
  fi

  CPU_REV=`grep "Processor Chip Revisions:" $TMP_CSTM | head -1 | awk '{print $8}'`
  CACHE1_I=`grep "Instruction Cache" $TMP_CSTM | head -1 | awk '{print $4}'`
  CACHE1_D=`grep "Data Cache" $TMP_CSTM | head -1 | awk '{print $4}'`
  CACHE2=`grep "2nd Level Cache Size:" $TMP_CSTM | head -1 | awk '{print $10}'`
  if [ "$CACHE2" = "N/A" ]; then
     CACHE2=0
  fi
  CPU_FAMILY_DESC=`echo Hewlett-Packard PA-RISC $CPU_MODEL Family Processors`
  if [ "$getconf_check" -eq 1 ] ; then
    BIT_LENGTH=`getconf HW_CPU_SUPP_BITS`
  fi
  if [ -z $BIT_LENGTH ] ; then
  	BIT_LENGTH=32
  fi

  # Print the same data for all CPU's
  let i=0;
  while [ $i -eq 0 ] || [ $i -lt "$NUM_CPUS" ] ; do
    let linestart=i+1
    echo '<CPU>'
    echo ISC_Identity=$i
    xx=`grep "Slot Number" $TMP_CSTM | head -$linestart | tail -1 | awk '{print $3}'`
    if [ -n "$xx" ] ; then
       echo ISC_SlotNumber=$xx
    else
       echo ISC_SlotNumber=$i
    fi

   xx=`grep "Serial Number" $TMP_CSTM | head -$linestart | tail -1 | awk '{print $3}'`
   if [ -n "$xx" ] ; then
      echo ISC_SerialNumber=$xx
   fi

    if [ -n "$CPU_CLOCKSPEED" ] ; then
       echo ISC_CurCPUSpeed=$CPU_CLOCKSPEED
    fi
    
    echo DCPU_Make=Hewlett-Packard Co.
    echo DCPU_Desc=$CPU_FAMILY_DESC

    echo DCPU_Family=$CPU_FAMILY
    echo DCPU_ModelNum=$CPU_MODEL
    echo DCPU_Stepping=$CPU_REV
    echo DCPU_Model=PA_RISC Family $CPU_FAMILY Model $CPU_MODEL Stepping $CPU_REV
    echo DCPU_RatedSpeed=$CPU_CLOCKSPEED
    echo DCPU_PrimCacheInst=$CACHE1_I
    echo DCPU_PrimCacheData=$CACHE1_D
    echo DCPU_SecCacheSize=$CACHE2
    echo DCPU_Architecture=$BIT_LENGTH
    echo '</CPU>'
    echo ""
    let i=i+1
  done
  rm -f $TMP_CSTM
}

do_cpu_hpux()
{
	if [ "$machinfo_check" -eq 1 ] ; then
		xx=`machinfo | grep 'Itanium'`
		if [ -n "$xx" ] ; then
			hpux_cpu_machinfo
			return
		fi
	fi

	if [ "$cstm_check" -eq 1 ] && [ "$MACH_TYPE" != "ia64" ] ; then
		hpux_cpu_cstm
		return
	fi
	
	if [ "$ioscan_check" -eq 1 ] ; then
    	    NUM_CPUS=`ioscan -fnkC processor | grep processor | wc -l`
	fi	
	if [ "$MACH_TYPE" != "ia64" ] ; then
		DCPU_Make='Hewlett-Packard Co.'
		if [ "$model_check" -eq 1 ] ; then
    			MODEL=`model|cut -d/ -f2`
			DCPU_ModelNum=`grep ^$MODEL /usr/sam/lib/mo/sched.models | awk '{print $3}'`
			DCPU_Stepping=`grep ^$MODEL /usr/sam/lib/mo/sched.models | awk '{print $2}'`
			if [ $MODEL -ge 800 ]; then
				DCPU_Family=1.0
			else
				DCPU_Family=1.1
			fi
			DCPU_Model=PA_RISC\ Family\ $DCPU_Family\ Model\ $DCPU_ModelNum\ Stepping\ $DCPU_Stepping
		fi
	else
		if [ "$model_check" -eq 1 ] ; then
			MODEL=`model|cut -d/ -f3`
		fi
		DCPU_Make=GenuineIntel
		DCPU_Family=31
	fi
	if [ "$adb_check" -eq 1 ] ; then
		if [ "$MACH_TYPE" != "ia64" ] ; then
			CPU_CLOCKSPEED=`echo "itick_per_usec/D"|adb -k /stand/vmunix /dev/kmem | tail -1 | awk '{print $2}'`
		else
			CPU_CLOCKSPEED=`echo "itick_per_usec/D"|adb -o /stand/vmunix /dev/kmem | tail -1 | awk '{print $2}'`
		fi
	fi

	if [ "$getconf_check" -eq 1 ] ; then
		BIT_LENGTH=`getconf HW_CPU_SUPP_BITS`
	fi
	if [ -z $BIT_LENGTH ] ; then
		BIT_LENGTH=32
	fi

	# Print the same data for all CPU's
	let i=0;
	while [ $i -eq 0 ] || [ $i -lt "$NUM_CPUS" ] ; do
		echo '<CPU>'
		echo ISC_Identity=$i
		echo ISC_SlotNumber=$i
		echo ISC_CurCPUSpeed=$CPU_CLOCKSPEED
		echo DCPU_Make=$DCPU_Make
		echo DCPU_Model=$DCPU_Model
		echo DCPU_RatedSpeed=$CPU_CLOCKSPEED
		echo DCPU_Architecture=$BIT_LENGTH
		echo '</CPU>'
		echo ""
		let i=i+1
	done
}

do_cpu_linux()
{
	if [ "$dmidecode_check" -eq 1 ] ; then
		DMIDECODE_INFO=$MYTMPDIR/dmidecode.$$
		if echo $sudocommands | grep -q ":dmidecode:" 
		then
			echo | sudo -S dmidecode > $DMIDECODE_INFO 2>/dev/null
		else
			dmidecode > $DMIDECODE_INFO 2>/dev/null
		fi
		xx=`cat $DMIDECODE_INFO | grep -A 45 "Processor Information" | head -45 | grep "External Clock"`
		DCPU_FSB=`expr "$xx" : ".*: * *\([0-9]*\)*"`
		xx=`cat $DMIDECODE_INFO | grep "L1 Instr Cache"`
		if [ -n "$xx" ] ; then
			DCPU_PrimCacheInst=`cat $DMIDECODE_INFO | grep -A 4 "L1 Instr" | grep "Installed Size:" | awk -F": " '{print $2}' | awk 'BEGIN {a=0;} {a=a+$1} END {print a}'`
			DCPU_PrimCacheData=`cat $DMIDECODE_INFO | grep -A 4 "L1 Data" | grep "Installed Size:" | awk -F": " '{print $2}' | awk 'BEGIN {a=0;} {a=a+$1} END {print a}'`
		else
			DCPU_PrimCacheSize=`cat $DMIDECODE_INFO | grep -A 3 "Level 1" | grep "Installed Size:" | awk -F": " '{print $2}' | awk 'BEGIN {a=0;} {a=a+$1} END {print a}'`
		fi
		DCPU_SecCacheSize=`cat $DMIDECODE_INFO | grep -A 3 "Level 2" | grep "Installed Size:" | awk -F": " '{print $2}' | awk 'BEGIN {a=0;} {a=a+$1} END {print a}'`
		DCPU_ThirdCacheSize=`cat $DMIDECODE_INFO | grep -A 3 "Level 3" | grep "Installed Size:" | awk -F": " '{print $2}' | awk 'BEGIN {a=0;} {a=a+$1} END {print a}'`
		rm -f $DMIDECODE_INFO
	fi

	if [ -z "$DCPU_PrimCacheInst" ] ; then
		if [ -r "$TMPDMESG" ] ; then
			DCPUCACHE=`grep "^.*CPU: L1" $TMPDMESG | head -1`
			if [ -n "$DCPUCACHE" ] ; then
				DCPU_PrimCacheInst=`expr "$DCPUCACHE" : ".*I [cC]ache: * *\([0-9]*\)*"`
				DCPU_PrimCacheData=`expr "$DCPUCACHE" : ".*D [cC]ache[: ]* *\([0-9]*\)*"`
			fi
		fi
	fi

	let CPUCNT=0
	while read F1 F2 F3 F4 ; do
		case "$F1" in
			processor)
				ISC_Identity=$F3
				;;
			vendor*)
				DCPU_Make=$F3
				[ -n "$F4" ] && DCPU_Make="$F3 $F4"
				;;
			cache)
				DCPU_SecCacheSize=`echo "$F4" | awk '{print $1}'`
				;;
			cpu)
				if [ "$F2" = "family" ] ; then
					DCPU_Family=$F4
				elif [ "$F2" = "MHz" ] ; then
					ISC_CurCPUSpeed=$F4
				elif [ "$F2" = "cores" ] ; then
					DCPU_Cores=$F4
				fi
				;;
			flags)
				DCPU_Flags="$F3 $F4"
				DCPU_BitLength=`echo "$DCPU_Flags" | awk 'BEGIN {FS=" ";} {for(i=1;i<=NF;i++) if ($i == "lm") {print 64; exit;} print 32}'`
				;;
			model)
				if [ "$F2" = "name" ] ; then
					DCPU_Desc=$F4
				else
					DCPU_ModelNum=$F3
				fi
				;;
			stepping)
				DCPU_Stepping=$F3
				;;
			physical)
				SOCKET=$F4
				;;
			core)
				CORE=$F4
				;;
			siblings)
				SIBLINGS=$F3
				;;
			[bB]ogo*)
				if [ -z "$DCPU_Cores" ] ; then
					DCPU_Hyperthreaded=0
				else
					CORE_UNIQUE=":"$SOCKET"-"$CORE":"
					CORE_EXISTS=`awk -v a=$CORE_LIST -v b=$CORE_UNIQUE 'BEGIN {print index(a, b)}'`
					if [ "$CORE_EXISTS" -gt 0 ] ; then
						# Ignoring the logical sibling CPU
						continue
					else
						CORE_LIST=":"$CORE_LIST":"$CORE_UNIQUE
						if [ "$SIBLINGS" -eq "$DCPU_Cores" ] ; then
							DCPU_Hyperthreaded=0
						else
							DCPU_Hyperthreaded=1
						fi
					fi
				fi

				if expr "$CPU_TYPE" : "i.*86" >/dev/null ; then
					ARCH=x86
				else
					ARCH=$CPU_TYPE
				fi
				if [ -z "$DCPU_BitLength" ] ; then
					DCPU_BitLength=`expr "$ARCH" : ".*[^0-9]\{1,\}\(64\)[^0-9]*$"`
					if [ -z $DCPU_BitLength ] ; then
						DCPU_BitLength=32
					fi
				fi

				echo '<CPU>'
				echo ISC_Identity=$ISC_Identity
				echo ISC_SlotNumber=$CPUCNT
				echo ISC_SerialNumber=$ISC_SerialNumber
				echo ISC_CurCPUSpeed=$ISC_CurCPUSpeed

				if [ -z "$DCPU_Cores" ] ; then
					echo DCPU_Cores=1
				else
					echo DCPU_Cores=$DCPU_Cores
				fi
				echo DCPU_Hyperthreaded=$DCPU_Hyperthreaded
				echo DCPU_Make=$DCPU_Make
				echo DCPU_Desc=$DCPU_Desc
				if [ -n "$DCPU_PrimCacheInst" ] ; then
					echo DCPU_PrimCacheInst=$DCPU_PrimCacheInst
				fi
				if [ -n "$DCPU_PrimCacheData" ] ; then
					echo DCPU_PrimCacheData=$DCPU_PrimCacheData
				fi
				if [ -n "$DCPU_PrimCacheSize" ] ; then
					echo DCPU_PrimCacheSize=$DCPU_PrimCacheSize
				fi
				if [ -n "$DCPU_SecCacheSize" ] ; then
					echo DCPU_SecCacheSize=$DCPU_SecCacheSize
				fi
				if [ -n "$DCPU_ThirdCacheSize" ] ; then
					echo DCPU_ThirdCacheSize=$DCPU_ThirdCacheSize
				fi
				echo DCPU_Family=$DCPU_Family
				echo DCPU_ModelNum=$DCPU_ModelNum
				echo DCPU_Stepping=$DCPU_Stepping
				echo DCPU_Model=$ARCH Family $DCPU_Family Model $DCPU_ModelNum Stepping $DCPU_Stepping
				echo DCPU_RatedSpeed=$ISC_CurCPUSpeed
				if [ -n "$DCPU_FSB" ] ; then
					echo DCPU_FSB=$DCPU_FSB
				fi
				echo DCPU_Flags=$DCPU_Flags
				echo DCPU_Architecture=$DCPU_BitLength
				echo '</CPU>'
				echo ""

				let CPUCNT=CPUCNT+1
			  ;;
		esac
	done < /proc/cpuinfo

	if [ -r /proc/vmware/sched/ncpus ] ; then
		LOGCPUS=`grep logical /proc/vmware/sched/ncpus | awk '{print $1}'`
		NUMCPUS=`grep cores /proc/vmware/sched/ncpus | awk '{print $1}'`
		PHYCPUS=`grep physical /proc/vmware/sched/ncpus | awk '{print $1}'`
		if [ -z "$PHYCPUS" ] ; then
			PHYCPUS=`grep packages /proc/vmware/sched/ncpus | awk '{print $1}'`
		fi
		if [ -z "$NUMCPUS" ] ; then
			NUMCPUS=$PHYCPUS
		else
			if [ -n "$LOGCPUS" ] ; then
				if [ $LOGCPUS -gt $NUMCPUS ] ; then
					DCPU_Hyperthreaded=1
				else
					DCPU_Hyperthreaded=0
				fi
			fi
			if [ -n "$PHYCPUS" ] && [ $PHYCPUS -gt 0 ] ; then
				let DCPU_Cores=NUMCPUS/PHYCPUS
			fi
		fi

		if [ $CPUCNT -gt 0 ] ; then
			while [ $CPUCNT -lt $NUMCPUS ] ; do
				let ISC_SlotNumber=CPUCNT
				echo '<CPU>'
				echo ISC_Identity=$ISC_SlotNumber
				echo ISC_SlotNumber=$ISC_SlotNumber
				echo ISC_SerialNumber=$ISC_SerialNumber
				echo ISC_CurCPUSpeed=$ISC_CurCPUSpeed

				if [ -z "$DCPU_Cores" ] ; then
					echo DCPU_Cores=1
				else
					echo DCPU_Cores=$DCPU_Cores
				fi
				if [ -n "$DCPU_Hyperthreaded" ] ; then
					echo DCPU_Hyperthreaded=$DCPU_Hyperthreaded
				fi
				echo DCPU_Make=$DCPU_Make
				echo DCPU_Desc=$DCPU_Desc
				if [ -n "$DCPU_PrimCacheInst" ] ; then
					echo DCPU_PrimCacheInst=$DCPU_PrimCacheInst
				fi
				if [ -n "$DCPU_PrimCacheData" ] ; then
					echo DCPU_PrimCacheData=$DCPU_PrimCacheData
				fi
				if [ -n "$DCPU_PrimCacheSize" ] ; then
					echo DCPU_PrimCacheSize=$DCPU_PrimCacheSize
				fi
				if [ -n "$DCPU_SecCacheSize" ] ; then
					echo DCPU_SecCacheSize=$DCPU_SecCacheSize
				fi
				if [ -n "$DCPU_ThirdCacheSize" ] ; then
					echo DCPU_ThirdCacheSize=$DCPU_ThirdCacheSize
				fi
				echo DCPU_Family=$DCPU_Family
				echo DCPU_ModelNum=$DCPU_ModelNum
				echo DCPU_Stepping=$DCPU_Stepping
				echo DCPU_Model=$ARCH Family $DCPU_Family Model $DCPU_ModelNum Stepping $DCPU_Stepping
				echo DCPU_RatedSpeed=$ISC_CurCPUSpeed
				echo DCPU_FSB=$DCPU_FSB
				echo DCPU_Flags=$DCPU_Flags
				echo DCPU_Architecture=$DCPU_BitLength
				echo '</CPU>'
				echo ""
				let CPUCNT=CPUCNT+1
			done
		fi
	fi
}

do_cpu_sun()
{
	TMP_CPUINFO=
	NUM_CPUS=1
	if [ "$psrinfo_check" -eq 1 ] ; then
		NUM_CPUS=`psrinfo | wc -l | tr -d ' '`
	fi
	if [ "$prtpicl_check" -eq 1 ] ; then
		TMP_CPUINFO=./cpuinfo.$$
		prtpicl -v -c cpu > $TMP_CPUINFO
	fi

	if [ -r "$TMP_CPUINFO" ]; then
		TEMP=`awk '/:ProcessorType/ {print $2; exit}' $TMP_CPUINFO`
		if [ "$TEMP" != "<ERROR:" ]; then
			CPU_TYPE=$TEMP
		fi
		if expr "$CPU_TYPE" : "i.*86" >/dev/null ; then
			ARCH=x86
		else
			ARCH=$CPU_TYPE
		fi

		if [ "$ARCH" = "x86" ]; then
			DCPU_Make=`awk '/:vendor-id/ {print $2; exit}' $TMP_CPUINFO`
			DCPU_Desc=`awk '/:brand-string/ {for(i=2;i<=NF;i++) print $i" "}' $TMP_CPUINFO`
			HEXNUM=`awk '/:l2-cache-size/ {print $2; exit}' $TMP_CPUINFO | tr '[:lower:]' '[:upper:]'`
			if  [ -n "$HEXNUM" ] ; then
				if expr "$HEXNUM" : "0X.*" >/dev/null ; then
					HEXNUM=`echo $HEXNUM | sed s/0X//`
					HEXNUM=`echo "ibase=16; $HEXNUM" | bc`
				fi
				DCPU_SecCacheSize=`expr "$HEXNUM" / 1024`
			fi
			HEXNUM=`awk '/:l1-icache-size/ {print $2; exit}' $TMP_CPUINFO | tr '[:lower:]' '[:upper:]'`
			if  [ -n "$HEXNUM" ] ; then
				if expr "$HEXNUM" : "0X.*" >/dev/null ; then
					HEXNUM=`echo $HEXNUM | sed s/0X//`
					HEXNUM=`echo "ibase=16; $HEXNUM" | bc`
				fi
				DCPU_PrimCacheInst=`expr "$HEXNUM" / 1024`
			fi
			HEXNUM=`awk '/:l1-dcache-size/ {print $2; exit}' $TMP_CPUINFO | tr '[:lower:]' '[:upper:]'`
			if  [ -n "$HEXNUM" ] ; then
				if expr "$HEXNUM" : "0X.*" >/dev/null ; then
					HEXNUM=`echo $HEXNUM | sed s/0X//`
					HEXNUM=`echo "ibase=16; $HEXNUM" | bc`
				fi
				DCPU_PrimCacheData=`expr "$HEXNUM" / 1024`
			fi
			DCPU_Family=`awk '/:family/ {print $2; exit}' $TMP_CPUINFO | tr '[:lower:]' '[:upper:]'`
			if expr "$DCPU_Family" : "0X.*" >/dev/null ; then
				DCPU_Family=`echo $DCPU_Family | sed s/0X//`
				DCPU_Family=`echo "ibase=16; $DCPU_Family" | bc`
			fi
			DCPU_ModelNum=`awk '/:cpu-model/ {print $2; exit}' $TMP_CPUINFO | tr '[:lower:]' '[:upper:]'`
			if expr "$DCPU_ModelNum" : "0X.*" >/dev/null ; then
				DCPU_ModelNum=`echo $DCPU_ModelNum | sed s/0X//`
				DCPU_ModelNum=`echo "ibase=16; $DCPU_ModelNum" | bc`
			fi
			DCPU_Stepping=`awk '/:stepping-id/ {print $2; exit}' $TMP_CPUINFO | tr '[:lower:]' '[:upper:]'`
			if expr "$DCPU_Stepping" : "0X.*" >/dev/null ; then
				DCPU_Stepping=`echo $DCPU_Stepping | sed s/0X//`
				DCPU_Stepping=`echo "ibase=16; $DCPU_Stepping" | bc`
			fi
			DCPU_Model="$DCPU_Make $ARCH Family $DCPU_Family Model $DCPU_ModelNum Stepping $DCPU_Stepping"
			DCPU_RatedSpeed=`awk '/:cpu-mhz/ {print $2; exit}' $TMP_CPUINFO | tr '[:lower:]' '[:upper:]'`
			if expr "$DCPU_RatedSpeed" : "0X.*" >/dev/null ; then
				DCPU_RatedSpeed=`echo $DCPU_RatedSpeed | sed s/0X//`
				DCPU_RatedSpeed=`echo "ibase=16; $DCPU_RatedSpeed" | bc`
			fi
			ISC_CurCPUSpeed=$DCPU_RatedSpeed
			DCPU_ProcessorID=`awk '/:cpuid-features/ {print $2; exit}' $TMP_CPUINFO | tr '[:lower:]' '[:upper:]'`
			if expr "$DCPU_ProcessorID" : "0X.*" >/dev/null ; then
				DCPU_ProcessorID=`echo $DCPU_ProcessorID | sed s/0X//`
				DCPU_ProcessorID=`echo "ibase=16; $DCPU_ProcessorID" | bc`
			fi
		else
			DCPU_Make='Sun Microsystems'
			DCPU_Desc=`awk '/:name/ {print substr($2,index($2,",")+1); exit}' $TMP_CPUINFO`
			HEXNUM=`awk '/:ecache-size/ {print substr($2,3); exit}' $TMP_CPUINFO | tr '[:lower:]' '[:upper:]'`
			DCPU_SecCacheSize=`echo "ibase=16; $HEXNUM/400" | bc`
			HEXNUM=`awk '/:icache-size/ {print substr($2,3); exit}' $TMP_CPUINFO | tr '[:lower:]' '[:upper:]'`
			DCPU_PrimCacheInst=`echo "ibase=16; $HEXNUM/400" | bc`
			HEXNUM=`awk '/:dcache-size/ {print substr($2,3); exit}' $TMP_CPUINFO | tr '[:lower:]' '[:upper:]'`
			DCPU_PrimCacheData=`echo "ibase=16; $HEXNUM/400" | bc`
			DCPU_Model="$DCPU_Make $ARCH"
			HEXNUM=`awk '/:clock-frequency/ {print substr($2,3); exit}' $TMP_CPUINFO | tr '[:lower:]' '[:upper:]'`
			DCPU_RatedSpeed=`echo "ibase=16; $HEXNUM/F4240" | bc`
			ISC_CurCPUSpeed=$DCPU_RatedSpeed
			if [ "$prtdiag_check" -eq 1 ] ; then
				DCPU_FSB=`prtdiag | grep -i frequency: | awk '{print $4}'`
			fi
		fi
	else
		ARCH=$CPU_TYPE
		DCPU_Make='Sun Microsystems'
		DCPU_Model="$DCPU_Make $ARCH"

		if [ "$psrinfo_check" -eq 1 ] ; then
			LINE=`psrinfo -v | grep operates | tail -1`
			DCPU_RatedSpeed=`echo "$LINE" | awk '{print $6}'`
		fi

		if [ "$prtdiag_check" -eq 1 ] ; then
			LINE=`prtdiag | head -1`
			DCPU_Desc=`echo "$LINE" | awk '{print $11}'`
			if [ -z "$DCPU_RatedSpeed" ] ; then
				DCPU_RatedSpeed=`expr "$LINE" : ".*\ \([0-9]*\)MHz"`
			fi
			if [ -z "$CPU_FAMILY_DESC" ] ; then
				DCPU_Desc=`expr "$LINE" : ".*\ \(.*\) [0-9]*MHz"`
			fi
			DCPU_FSB=`prtdiag | grep -i frequency: | awk '{print $4}'`
		fi

		ISC_CurCPUSpeed=$DCPU_RatedSpeed	
	fi

	if [ "$isainfo_check" -ne 0 ] ; then
		DCPU_BitLength=`isainfo -b 2>/dev/null`
	fi
	if [ -z $DCPU_BitLength ] ; then
		DCPU_BitLength=`expr "$ARCH" : ".*[^0-9]\{1,\}\(64\)[^0-9]*$"`
		if [ -z $DCPU_BitLength ] ; then
			DCPU_BitLength=32
		fi
	fi

	CPUCNT=0
	while [ $CPUCNT -lt "$NUM_CPUS" ] ; do
		echo '<CPU>'
		echo ISC_Identity=$CPUCNT
		echo ISC_SlotNumber=$CPUCNT
		echo DCPU_Make=$DCPU_Make

		if [ -n "$DCPU_Desc" ]; then
			echo DCPU_Desc=$DCPU_Desc
		fi
		if [ -n "$DCPU_SecCacheSize" ]; then
			echo DCPU_SecCacheSize=$DCPU_SecCacheSize
		fi
		if [ -n "$DCPU_PrimCacheInst" ]; then
			echo DCPU_PrimCacheInst=$DCPU_PrimCacheInst
		fi
		if [ -n "$DCPU_PrimCacheData" ]; then
			echo DCPU_PrimCacheData=$DCPU_PrimCacheData
		fi
		if [ -n "$DCPU_Family" ]; then
			echo DCPU_Family=$DCPU_Family
		fi
		if [ -n "$DCPU_ModelNum" ]; then
			echo DCPU_ModelNum=$DCPU_ModelNum
		fi
		if [ -n "$DCPU_Stepping" ]; then
			echo DCPU_Stepping=$DCPU_Stepping
		fi
		if [ -n "$DCPU_Model" ]; then
			echo DCPU_Model=$DCPU_Model
		fi
		if [ -n "$DCPU_RatedSpeed" ]; then
			echo DCPU_RatedSpeed=$DCPU_RatedSpeed
		fi
		if [ -n "$ISC_CurCPUSpeed" ]; then
			echo ISC_CurCPUSpeed=$ISC_CurCPUSpeed
		fi
		if [ -n "$DCPU_Flags" ]; then
			echo DCPU_Flags=$DCPU_Flags
		fi
		if [ -n "$DCPU_ProcessorID" ]; then
			echo DCPU_ProcessorID=$DCPU_ProcessorID
		fi
		if [ -n "$DCPU_FSB" ]; then
			echo DCPU_FSB=$DCPU_FSB
		fi
		if [ -n "$DCPU_BitLength" ]; then
			echo DCPU_Architecture=$DCPU_BitLength
		fi
		echo '</CPU>'
		echo ""
		CPUCNT=`expr $CPUCNT + 1`
	done
	if [ -r "$TMP_CPUINFO" ] ; then
		rm -f $TMP_CPUINFO
	fi
}

do_cpu_aix()
{
	[ "$lsdev_check" -eq 0 ] && return

	if [ "$getconf_check" -eq 1 ] ; then
		DCPU_BitLength=`getconf HARDWARE_BITMODE 2>/dev/null`
	fi
	if [ -z "$DCPU_BitLength" ] ; then
		if [ "$bootinfo_check" -eq 1 ] ; then
			DCPU_BitLength=`bootinfo -y 2>/dev/null`
		fi
		if [ -z $DCPU_BitLength ] ; then
			DCPU_BitLength=32
		fi
	fi

	let CPUCNT=0
	lsdev -Cc processor 2>/dev/null |
	while read INST STATUS HWPATH DESC ; do
		echo '<CPU>'
		echo ISC_Identity=$INST
		echo ISC_SlotNumber=$CPUCNT
		echo DCPU_Make="IBM"
		Major_Ver=`uname -v`
		Minor_Ver=`uname -r`
		if [ "$lsattr_check" -eq 1 ] ; then
			echo DCPU_Model=`lsattr -E -l $INST | grep "^type" | awk '{print $2}'`
			if [ \( "$Major_Ver" -ge 5 \) -o \( \( "$Major_Ver" -eq 5 \) -a \( "$Minor_Ver" -ge 1 \) \) ] ; then
				echo DCPU_RatedSpeed=`lsattr -E -l proc0 | grep "Processor Speed" | awk '{print $2 / 1000000}'`
			fi
		fi
		echo DCPU_Architecture=$DCPU_BitLength
		echo '</CPU>'
		echo ""
		CPUCNT=`expr $CPUCNT + 1`
	done
}

do_cpu()
{
	case "$OS_TYPE" in
	  HP-UX)
		 do_cpu_hpux
		 ;;
	  Linux)
		 do_cpu_linux
		 ;;

	  SunOS)
		 do_cpu_sun
		 ;;
	  AIX)
		 do_cpu_aix
		 ;;
	esac
}

# **************************************************************************
#  Drive Adapter Information Section
# **************************************************************************
do_driveadpt_linux()
{
	if [ -e /proc/scsi/scsi ]; then
		set +f
		SCSI_LIST=`echo /proc/scsi/*/[0-9]* 2>/dev/null`
		set -f
		for SCSI in $SCSI_LIST ; do
			if [ "$SCSI" != "/proc/scsi/*/[0-9]*" ]; then
				echo '<DRIVEADPT>'
				echo DDA_Type=SCSI
				echo ISDA_Identity=scsi`echo $SCSI | awk -F/ '{print $5}'`
				echo ISDA_BusNumber=`echo $SCSI | awk -F/ '{print $5}'`
				SCSIMOD=`head -1 $SCSI`
				SCSIDEVID=`head -3 $SCSI | tail -1 | sed 's/^.*Id=//' | sed 's/,.*$//'`
				SCSIPARAMS=`grep -A 1 "Target $SCSIDEVID" $SCSI | tail -1`
				echo DDA_Model=$SCSIMOD
				if expr "$SCSIMOD" : ".*emulation.*" >/dev/null ; then
					echo DDA_Make="SCSI emulation"
				else
					echo DDA_Make=`echo $SCSIMOD | awk '{print $2}' | cut -f1 -d ","`
				fi
				if [ -n "$SCSIPARAMS" ]; then
					echo DDA_MaxSpeed=`echo $SCSIPARAMS | awk '{print $2}' | awk -F. '{print $1}'`
				fi
				echo '</DRIVEADPT>'
				echo ""
			fi
		done
	fi

	if [ -e /proc/ide ]; then
		set +f
		IDE_LIST=`echo /proc/ide/ide* 2>/dev/null`
		set -f
		for IDE in $IDE_LIST ; do
			if [ "$IDE" != "/proc/ide/ide*" ]; then
				echo '<DRIVEADPT>'
				echo DDA_Type=IDE
				echo DDA_Model=`cat $IDE/model`
				echo ISDA_Identity=$IDE
				echo ISDA_BusNumber=`cat $IDE/channel`
				echo '</DRIVEADPT>'
				echo ""
			fi
		done
	fi

	set +f
	DRVLIST=`echo /proc/driver/cciss/cciss*`
	set -f
	[ "$DRVLIST" == "/proc/driver/cciss/cciss*" ] && return

	for DRV in $DRVLIST ; do
		CNTRLR=`echo $DRV | awk -F / '{print $NF}'`
		DDR_Model=`grep "^$CNTRLR" $DRV | sed s/"$CNTRLR: "//`
		echo '<DRIVEADPT>'
		echo DDA_Make="HP"
		echo DDA_Model=$DDR_Model
		echo DDA_Type=SCSI
		echo ISDA_Identity=$CNTRLR
		echo '</DRIVEADPT>'
		echo ""
	done
}

do_driveadpt_hpux()
{
	[ "$ioscan_check" -eq 0 ] && return
	TMP_DISKINFO=$MYTMPDIR/diskinfo.$$

	ioscan -FFkC ext_bus 2>/dev/null |
	while read ADAPT; do
		echo '<DRIVEADPT>'
		DSKTYPE=`echo $ADAPT | awk -F: '{print $10}'`
		if [ "$DSKTYPE" = "side" ] ; then
			DSKTYPE=IDE
		elif  [ "$DSKTYPE" = "mpt" ] ; then
			DSKTYPE=SCSI
		elif  [ "$DSKTYPE" = "c720" ] ; then
			DSKTYPE=SCSI
		fi
		echo DDA_Type=$DSKTYPE
		ISDAIDENT=`echo $ADAPT | awk -F: '{print $11}'`
		echo ISDA_Identity=$ISDAIDENT
		echo DDA_Make=HP
		echo DDA_Model=`echo $ADAPT | awk -F: '{print $18}'`
		echo '</DRIVEADPT>'
		echo ""
	done
	rm -f $TMP_DISKINFO
}

do_driveadpt_sun()
{
	ADPT_FOUND=0
	if [ "$prtdiag_check" -eq 1 ] ; then
		prtdiag | egrep 'ide-|scsi-' | 
		while read ADPT; do
			ADPT_FOUND=1
			echo '<DRIVEADPT>'
			echo DDA_Type=SCSI
			echo ISDA_Identity=`echo $ADPT | awk '{print $4,$5}'`
			echo ISDA_BusNumber=`echo $ADPT | awk '{print $4}'`
			echo DDA_Make=`echo $ADPT | awk '{print $5}'`
			echo '</DRIVEADPT>'
			echo ""
		done
	fi
	
	if [ "$ADPT_FOUND" -eq 0 ] && [ "$prtpicl_check" -eq 1 ] ; then
		TMP_ADPTINFO=./adptinfo.$$
		prtpicl -v -c scsi > $TMP_ADPTINFO
		if [ -r "$TMP_ADPTINFO" ]; then
			DDA_Type=SCSI
			ISDA_Identity=`head -1 $TMP_ADPTINFO`
			ISDA_BusNumber=`awk '/:bus-addr/ {print $2; exit}' $TMP_ADPTINFO`
			DDA_Make=`awk '/:manufacturer/ {print $2, $3, $4, $5, $6, $7, $8, $9, $10; exit}' $TMP_ADPTINFO`
			DDA_MODEL=`awk '/:version/ {print $2, $3, $4, $5, $6, $7, $8, $9, $10; exit}' $TMP_ADPTINFO`
			#All those fake columns in the awk print statement handle the case where the model or manufacturer have spaces in the name and different parts are considered different columns by awk
			#E.g. MRMP Virtual SCSI Controller would be considered 4 collumn table
			if [ -n "$ISDA_Identity" ] ; then
				ADPT_FOUND=2
			fi
		fi
		rm -f $TMP_ADPTINFO
	fi
	
	#Handle SUN system with Fibre Channel drive adapter
	if [ "$ADPT_FOUND" -eq 0 ] && [ "$prtpicl_check" -eq 1 ] ; then
		TMP_ADPTINFO=./adptinfo.$$
		prtpicl -v -c scsi-fcp > $TMP_ADPTINFO
		if [ -r "$TMP_ADPTINFO" ]; then
			DDA_Type=SCSI
			ISDA_Identity=`head -1 $TMP_ADPTINFO`
			ISDA_BusNumber=`awk '/:bus-addr/ {print $2; exit}' $TMP_ADPTINFO`
			DDA_Make=`awk '/:manufacturer/ {print $2, $3, $4, $5, $6, $7, $8, $9, $10; exit}' $TMP_ADPTINFO`
			DDA_MODEL=`awk '/:version/ {print $2, $3, $4, $5, $6, $7, $8, $9, $10; exit}' $TMP_ADPTINFO`
			
			if [ -n "$ISDA_Identity" ] ; then
				ADPT_FOUND=2
			fi
		fi
		rm -f $TMP_ADPTINFO
	fi
	
	if [ "$ADPT_FOUND" -eq 2 ] ; then
		echo '<DRIVEADPT>'
		echo DDA_Type=$DDA_Type
		echo ISDA_Identity=$ISDA_Identity
		echo ISDA_BusNumber=$ISDA_BusNumber
		echo DDA_Make=$DDA_Make
		echo DDA_Model=$DDA_MODEL
		echo '</DRIVEADPT>'
		echo ""
	fi
}

do_driveadpt_aix()
{
	[ "$lsparent_check" -eq 0 ] && return
	lsparent -C -k scsi_scb && lsparent -C -k lsa && lsparent -C -k wsa && lsparent -C -k scsi |
	while read ADPTID ADPTSTATUS ADPTCHAN ADPTMODEL; do
		echo '<DRIVEADPT>'
		echo DDA_Type=SCSI
		echo ISDA_Identity=$ADPTID
		echo ISDA_BusNumber=`echo $ADPTCHAN | awk -F- '{print $1}'`
		echo DDA_Make=Unknown
		echo DDA_Model=$ADPTMODEL
		echo '</DRIVEADPT>'
		echo ""
	done
}

do_driveadpt()
{
	case "$OS_TYPE" in
	  HP-UX)
		 do_driveadpt_hpux
		 ;;
	  Linux)
		 do_driveadpt_linux
		 ;;
	  SunOS)
		 do_driveadpt_sun
		 ;;
	  AIX)
		 do_driveadpt_aix
		 ;;
	esac
}

# **************************************************************************
#  Drive Information Section
# **************************************************************************
do_drive_linux_ide()
{
	set +f
	IDE_LIST=`echo /proc/ide/ide* 2>/dev/null`
	set -f
	for IDE in $IDE_LIST ; do
		if [ "$IDE" != "/proc/ide/ide*" ]; then
			set +f
			DSK_LIST=`echo $IDE/hd* 2>/dev/null`
			set -f
			for DSK in $DSK_LIST ; do
				if [ "$DSK" != "$IDE/hd*" ]; then
					set -- `cat $DSK/model`
					VENDOR=$1
					MODEL=$2

					# Look for a separating hyphen
					if [ -z "$MODEL" ] ; then
						MODEL=`echo $VENDOR | cut -d- -f2-`
						if [ -n "$MODEL" ] ; then
							VENDOR=`echo $VENDOR | cut -d- -f1`
						fi
					fi

					echo "<DISKINFO>"
					if [ "$basename_check" -eq 1 ] ; then
						ISDR_Identity=`basename $DSK`
					fi
					echo ISDR_Identity=$ISDR_Identity
					echo ISDR_Type=`cat $DSK/media`
					echo ISDR_ISDA_Identity=$IDE
					echo DDR_IntType=IDE
					echo DDR_Make=$VENDOR
					echo DDR_Model=$MODEL

					DDR_Space=
					if [ -r $DSK/capacity ]; then
						DDR_Space=`cat $DSK/capacity 2>/dev/null`
						let DDR_Space=DDR_Space/1000*512/1000
					fi
					if [ -z $DDR_Space ] ; then
						DDR_Space=`grep $ISDR_Identity:.*sectors $TMPDMESG | cut -d ' ' -f 4 | sed s/\(// 2>/dev/null`
					fi
					if [ -n $DDR_Space ] ; then
						echo DDR_Space=$DDR_Space
						echo ISDR_Space=$DDR_Space
					fi

					DDR_Cache=
					if [ -r $DSK/cache ] ; then
						echo DDR_Cache=`cat $DSK/cache`
					fi

					DDR_TotalCyl=
					DDR_TotalHeads=
					DDR_TotalSectors=
					if [ -r $DSK/geometry ] ; then
						GEOM=`cat $DSK/geometry | grep ^logical | awk '{print $2}'`
						DDR_TotalCyl=`echo $GEOM | awk -F/ '{print $1}'`
						DDR_TotalHeads=`echo $GEOM | awk -F/ '{print $2}'`
						DDR_TotalSectors=`echo $GEOM | awk -F/ '{print $3}'`
					fi
					if [ -r $DSK/settings ] ; then
						DDR_TotalCyl=`cat $DSK/settings | grep ^bios_cyl | awk '{print $2}'`
						DDR_TotalHeads=`cat $DSK/settings | grep ^bios_head | awk '{print $2}'`
						DDR_TotalSectors=`cat $DSK/settings | grep ^bios_sec | awk '{print $2}'`
					fi

					if [ -z "$DDR_TotalCyl" ] ; then
						DDR_TotalCyl=`grep $ISDR_Identity:.*sectors $TMPDMESG | cut -d ' ' -f 8 | cut -d '/' -f 1 | cut -d '=' -f 2 2>/dev/null`
						let DDR_Space=DDR_Space/1000
					fi
					if [ -n "$DDR_TotalCyl" ] ; then
						echo DDR_TotalCyl=$DDR_TotalCyl
					fi
					if [ -z "$DDR_TotalHeads" ] ; then
						DDR_TotalHeads=`grep $ISDR_Identity:.*sectors $TMPDMESG | cut -d ' ' -f 8 | cut -d '/' -f 2 2>/dev/null`
					fi
					if [ -n "$DDR_TotalHeads" ] ; then
						echo DDR_TotalHeads=$DDR_TotalHeads
					fi
					if [ -z "$DDR_TotalSectors" ] ; then
						DDR_TotalSectors=`grep $ISDR_Identity:.*sectors $TMPDMESG | cut -d ' ' -f 2 2>/dev/null`
					fi
					if [ -n "$DDR_TotalSectors" ] ; then
						echo DDR_TotalSectors=$DDR_TotalSectors
					fi

					if [ -r $DSK/settings ] ; then
						INTSPEED=`cat $DSK/settings | grep ^current_speed`
						echo ISDR_CurIntSpeed=`echo $INTSPEED | awk '{print $2}'`
					fi
					echo "</DISKINFO>"
					echo ""
				fi
			done
		fi
	done
}

get_disk_geom()
{
	if [ "$fdisk_check" -eq 1 ] ; then
		# Try fdisk
		GEOM=`fdisk -l -u $DEVPATH 2>/dev/null | grep heads`
		if [ -n "$GEOM" ] ; then
			BPS=`fdisk -l -u $DEVPATH 2>/dev/null | grep Units`
			DDR_BytesPerSec=`expr "$BPS" : ".*\ \([0-9]*\) bytes"`
			DDR_SectorsPerTrack=`expr "$GEOM" : '.*, \([0-9]*\) sectors' 2>/dev/null`
			DDR_TotalCyl=`expr "$GEOM" : '.*, \([0-9]*\) cylinders' 2>/dev/null`
			DDR_TotalHeads=`expr " $GEOM" : '.* \([0-9]*\) heads' 2>/dev/null`
			DDR_TotalSectors=`expr $DDR_SectorsPerTrack \* $DDR_TotalCyl \* $DDR_TotalHeads 2>/dev/null`
			if [ -n "$DDR_TotalSectors" ] && [ -n "$DDR_BytesPerSec" ] ; then
				let DDR_Space=DDR_TotalSectors/1000*DDR_BytesPerSec/1000
			fi
		fi
	fi

	if [ "$hdparm_check" -eq 1 ] ; then
		# Try hdparm
		GEOM=`hdparm -g $DEVPATH 2>/dev/null | grep geometry | sed 's/^.*geometry[\ ]*=\ //'`
		if [ -n "$GEOM" ] ; then
			DDR_BytesPerSec=512
			if [ -z "$DDR_SectorsPerTrack" ] ; then
				DDR_SectorsPerTrack=`expr "$GEOM" : '.*/\([0-9]*\),' 2>/dev/null`
			fi
			if [ -z "$DDR_TotalCyl" ] ; then
				DDR_TotalCyl=`expr "$GEOM" : '\([0-9]*\)/' 2>/dev/null`
			fi
			if [ -z "$DDR_TotalHeads" ] ; then
				DDR_TotalHeads=`expr "$GEOM" : '.*/\([0-9]*\)/' 2>/dev/null`
			fi
			if [ -z "$DDR_TotalSectors" ] ; then
				DDR_TotalSectors=`expr "$GEOM" : '.*sectors = \([0-9]*\),' 2>/dev/null`
			fi
			if [ -z "$DDR_Space" ] ; then
				if [ -n "$DDR_TotalSectors" ] ; then
					let DDR_Space=DDR_TotalSectors/1000*DDR_BytesPerSec/1000
				fi
			fi
		fi
	fi

	if [ -z "$GEOM" ] && [ -r "$TMPDMESG" ] ; then
		# Try dmesg
		GEOM=`grep "^SCSI device $DEVNAME" $TMPDMESG | head -1`
		# Try another string pattern
		if [ -z "$GEOM" ] ; then
			GEOM=`grep "$DEVNAME.*sectors" $TMPDMESG | tail -1`
		fi
		if [ -n "$GEOM" ] ; then
			DDR_TotalSectors=`echo $GEOM | awk '{print $4}'`
			DDR_BytesPerSec=`echo $GEOM | awk '{print $5}' | sed 's/-byte//'`
			let DDR_Space=DDR_TotalSectors/1000*DDR_BytesPerSec/1000
		fi
	fi

	if [ -z "$DDR_Space" ] && [ -r /proc/partitions ] ; then
		# Try /proc/partions
		GEOM=`grep " $DEVNAME" /proc/partitions | head -1`
		if [ -n "$GEOM" ] ; then
			DDR_Space=`echo $GEOM | awk '{print $3*1024/1000/1000}'`
		fi
	fi

	if [ -n "$DDR_BytesPerSec" ] ; then
		echo DDR_BytesPerSec=$DDR_BytesPerSec
	fi
	if [ -n "$DDR_SectorsPerTrack" ] ; then
		echo DDR_SectorsPerTrack=$DDR_SectorsPerTrack
	fi
	if [ -n "$DDR_TotalCyl" ] ; then
		echo DDR_TotalCyl=$DDR_TotalCyl
	fi
	if [ -n "$DDR_TotalHeads" ] ; then
		echo DDR_TotalHeads=$DDR_TotalHeads
	fi
	if [ -n "$DDR_TotalSectors" ] ; then
		echo DDR_TotalSectors=$DDR_TotalSectors
	fi
	if [ -n "$DDR_Space" ] ; then
		echo DDR_Space=$DDR_Space
		echo ISDR_Space=$DDR_Space
	fi
}

do_drive_linux_scsi()
{
	[ ! -e /proc/scsi/scsi ] && return
	TMP_DISKINFO=$MYTMPDIR/diskinfo.$$
	IDENTITY=
	VENDOR=
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
				GEOM=
				ISDR_CurIntSpeed=
				DDR_BytesPerSec=
				DDR_SectorsPerTrack=
				DDR_TotalCyl=
				DDR_TotalHeads=
				DDR_TotalSectors=
				DDR_TotalTracks=
				DDR_TracksPerCyl=
				DDR_Space=
				
				SCSIBUS=`echo $SCSINAME | sed 's/scsi//'`
				set +f
				SCSIPARAMS=`grep -A 3 "^Target $DEVID " /proc/scsi/*/$SCSIBUS 2>/dev/null | tail -1`
				set -f
				if [ -n "$SCSIPARAMS" ] ; then
					ISDR_CurIntSpeed=`echo $SCSIPARAMS | awk '{print $2}' | awk -F. '{print $1}'`
				fi
				let DEVNUM=97+DEVID
				let DEVID=DEVID+1
				DEVNAME=`echo $DEVNUM | awk '{printf("sd%c",$1)}'`
				DEVPATH=/dev/$DEVNAME

				echo "<DISKINFO>"
				echo ISDR_Identity=$IDENTITY
				echo ISDR_ISDA_Identity=$SCSINAME
				echo ISDR_DeviceId=$DEVID
				echo ISDR_LUN=$ISDR_LUN
				if [ -n "$SCSIPARAMS" ] ; then
					echo ISDR_CurIntSpeed=`echo $SCSIPARAMS | awk '{print $2}' | awk -F. '{print $1}'`
				fi
				echo DDR_IntType=SCSI
				echo DDR_Make=$VENDOR
				echo DDR_Model=$MODEL

				get_disk_geom

				echo "</DISKINFO>"
				echo ""
			fi
		
			VENDOR=
			set -- $LINE
			IDENTITY=$2:c$4:$6:l$8
			SCSINAME=$2
			# DEVID=`echo $6 | awk '{print $1 * 1}'`
			ISDR_LUN=$8
		else
			if expr "$LINE" : Vendor >/dev/null ; then
				VENDOR=`echo "$LINE" | sed -e 's/Vendor: *//' -e 's/ *Model.*//'`
				MODEL=`echo "$LINE" | cut -d: -f3 | sed -e 's/^ *//' -e 's/ *Rev.*//'`
			elif expr "$LINE" : Type >/dev/null ; then
				TYPE=`echo "$LINE" | sed -e 's/Type: *//' -e 's/ *ANSI.*//'`
			fi
		fi
	done
  )
  rm -f $TMP_DISKINFO
}

do_drive_linux_cciss()
{
	set +f
	DRVLIST=`echo /proc/driver/cciss/cciss*`
	set -f
	[ "$DRVLIST" == "/proc/driver/cciss/cciss*" ] && return

	for DRV in $DRVLIST ; do
		CNTRLR=`echo $DRV | awk -F / '{print $NF}'`
		DDR_Model=`grep "^$CNTRLR" $DRV | sed s/"$CNTRLR: "//`

		DSKLIST=`grep cciss/ $DRV | awk -F : '{print $1}'`
		for DEVNAME in $DSKLIST ; do
			GEOM=
			DDR_BytesPerSec=
			DDR_SectorsPerTrack=
			DDR_TotalCyl=
			DDR_TotalHeads=
			DDR_TotalSectors=
			DDR_TotalTracks=
			DDR_TracksPerCyl=
			DDR_Space=

			DEVPATH=/dev/$DEVNAME

			echo "<DISKINFO>"
			echo ISDR_Identity=`echo $DEVNAME | sed s/"cciss\/"//`
			echo ISDR_ISDA_Identity=$CNTRLR
			echo DDR_IntType=SCSI
			echo DDR_Make=HP
			echo DDR_Model=$DDR_Model
			get_disk_geom
			echo "</DISKINFO>"
			echo ""
		done	
	done
}

do_drive_hpux()
{
	TMP_DISKINFO=$MYTMPDIR/diskinfo.$$

	[ "$ioscan_check" -eq 0 ] && return
	ioscan -FFkC ext_bus 2>/dev/null |
	while read ADAPT; do
		DSKTYPE=`echo $ADAPT | awk -F: '{print $10}'`
		if [ "$DSKTYPE" = "side" ] ; then
			DSKTYPE=IDE
		elif  [ "$DSKTYPE" = "mpt" ] ; then
			DSKTYPE=SCSI
		elif  [ "$DSKTYPE" = "c720" ] ; then
			DSKTYPE=SCSI
		fi
		ISDAIDENT=`echo $ADAPT | awk -F: '{print $11}'`

		DISKS=`ioscan -kd sdisk | grep ^$ISDAIDENT | cut -f 1 -d " "`
		for DISK in $DISKS; do
			DISKDEV=`ioscan -FknH $DISK | head -2 | tail -1`
			DSK=`echo $DISKDEV | awk '{print $1}'`
			RDSK=`echo $DSK | sed s,/dev/dsk/,/dev/rdsk/,`
			DSKIDENT=`echo $DSK | sed s,/dev/dsk/,,`

			diskinfo -v $RDSK > $TMP_DISKINFO 2>/dev/null
			echo '<DISKINFO>'
			echo ISDR_Identity=$DSKIDENT
			echo ISDR_Type=`grep "type:" $TMP_DISKINFO | awk '{print $2}'`
			echo ISDR_ISDA_Identity=`echo $ADAPT | awk -F: '{print $11}'`
			echo ISDR_LUN=`expr $DSKIDENT : '.*t\(.*\)d.*'`
			echo DDR_IntType=$DSKTYPE
			echo DDR_Make=`grep "vendor:" $TMP_DISKINFO | awk '{print $2}'`
			echo DDR_Model=`grep "product id:" $TMP_DISKINFO | awk '{print $3}'`
			DDR_Space=`grep "size:" $TMP_DISKINFO | awk '{print $2}'`
			let DDR_Space=DDR_Space/1000
			echo DDR_Space=$DDR_Space
			echo ISDR_Space=$DDR_Space
			echo DDR_TotalSectors=`grep "blocks per disk:" $TMP_DISKINFO | awk '{print $4}'`
			echo '</DISKINFO>'
			echo ""
		done
	done
	rm -f $TMP_DISKINFO
}

do_drive_sun()
{
	[ "$df_check" -eq 0 ] && return

	TMP_DISKINFO=./diskinfo.$$
	PDISK=
	df -lk | grep '^/dev/dsk/' 2>/dev/null |
	while read LINE; do
		RDEV=`echo $LINE | awk '{print $1}' | sed s,/dev/dsk/,/dev/rdsk/, | awk -F: '{print $1}'`
		DISK=`echo $RDEV | cut -c1-16`

		if [ "$DISK" != "$PDISK" ]; then
			PDISK=$DISK
			echo '<DISKINFO>'
			if [ "$basename_check" -eq 1 ] ; then
				ISDR_Identity=`basename $DISK`
				echo ISDR_Identity=$ISDR_Identity
				if [ "$iostat_check" -eq 1 ] ; then
					LineNo=`iostat -En | grep -n $ISDR_Identity | awk -F: '{print "sed -n '\''"$1+1","$1+1"p'\''"}'`
					Line=`iostat -En | eval $LineNo`
					ISDR_SerialNumber=`expr "$Line" : '.*Serial No: *\([^ ]*\)'`
					if [ -n "$ISDR_SerialNumber" ] ; then
						echo ISDR_SerialNumber=$ISDR_SerialNumber
						fi
					DDR_Make=`expr "$Line" : '.*Vendor: *\([^ ]*\)'`
					if [ -n "$DDR_Make" ] ; then
						echo DDR_Make=$DDR_Make
					else
						echo DDR_Make=Unknown
					fi
				else
					echo DDR_Make=Unknown
				fi
			else
				echo DDR_Make=Unknown
			fi
			echo DDR_Model=Unknown
			echo DDR_IntType=HD

			if [ "$prtvtoc_check" -eq 1 ] ; then
				prtvtoc $RDEV > $TMP_DISKINFO 2>/dev/null
				BPS=`grep 'bytes/sector' $TMP_DISKINFO | awk '{print $2}'`
				SPT=`grep 'sectors/track' $TMP_DISKINFO | awk '{print $2}'`
				TPC=`grep 'tracks/cylinder' $TMP_DISKINFO | awk '{print $2}'`
				TOTC=`grep 'cylinders' $TMP_DISKINFO | grep -v accessible | awk '{print $2}'`
			else
				BPS=0
				SPT=0
				TPC=0
				TOTC=0
			fi

			echo DDR_BytesPerSec=$BPS
			echo DDR_SectorsPerTrack=$SPT
			echo DDR_TotalCyl=$TOTC
			TMPVAR=`expr $SPT \* $TPC \* $TOTC`
			echo DDR_TotalSectors=$TMPVAR
			TMPVAR=`expr $TPC \* $TOTC`
			echo DDR_TotalTracks=$TMPVAR
			echo DDR_TracksPerCyl=$TPC
			TMPVAR=`expr $SPT \* $TPC \* $TOTC  \* $BPS \/ 1000 \/ 1000`
			echo DDR_Space=$TMPVAR
			echo ISDR_Space=$TMPVAR
			echo '</DISKINFO>'
			echo ""
		fi
	done
	rm -f $TMP_DISKINFO
}

do_drive_aix()
{
	[ "$lsdev_check" -eq 0 ] && return
	TMP_DISKINFO=./diskinfo.$$
	lsdev -Cc disk 2>/dev/null |
	while read INST STATUS HWPATH DESC; do
		echo '<DISKINFO>'
		if [ "$lscfg_check" -eq 1 ] ; then
			lscfg -v -l $INST > $TMP_DISKINFO 2>/dev/null
			SERIAL=`cat $TMP_DISKINFO | grep Serial | awk -F. '{print $NF}'`
			if [ -n "$SERIAL" ] ; then
				IDENT=$INST:$SERIAL
			fi
			echo DDR_Make=`cat $TMP_DISKINFO | grep Manufacturer | awk -F. '{print $NF}'`
			echo DDR_Model=`cat $TMP_DISKINFO | grep Model | awk -F. '{print $NF}'`
		fi
		if [ -z "$SERIAL" ] ; then
			IDENT=$INST
		fi
		echo ISDR_Identity=$IDENT
		echo ISDR_DeviceId=`echo $HWPATH | cut -f 4 -d "-" | cut -f 1 -d ","`
		echo ISDR_SerialNumber=$SERIAL
		echo DDR_IntType=HD
		if [ "$lsparent_check" -eq 1 ] ; then
			echo ISDR_ISDA_Identity=`lsparent -C -l $INST | awk '{print $1}'`
		fi
		TMPVAR=
		if [ "$lsattr_check" -eq 1 ] ; then
			TMPVAR=`lsattr -E -l $INST | grep size_in_mb | awk '{print $2}'`
		fi
		if [ -z "$TMPVAR" ] ; then
			if [ "$bootinfo_check" -eq 1 ] ; then
				TMPVAR=`bootinfo -s $INST`
			fi
		fi
		if [ -n "$TMPVAR" ] ; then
			echo DDR_Space=$TMPVAR
			echo ISDR_Space=$TMPVAR
		fi
		echo '</DISKINFO>'
		echo ""
		rm -f $TMP_DISKINFO
	done
}

do_drive()
{
	case "$OS_TYPE" in
	  HP-UX)
		 do_drive_hpux
		 ;;
	  Linux)
		 do_drive_linux_ide
		 do_drive_linux_scsi
		 do_drive_linux_cciss
		 ;;
	  SunOS)
		 do_drive_sun
		 ;;
	  AIX)
		 do_drive_aix
		 ;;
	esac
}

# **************************************************************************
#  NIC Information Section
# **************************************************************************
hpux_one_lan()
{
  if [ $# -ne 1 ] || [ -z "$1" ]; then
    return
  fi
  LANID=$1

  echo '<NETWORK>'

  echo DNIC_Make=HP

  if [ "$lanadmin_check" -eq 1 ] ; then
    # Ethernet address
    MACADDR=`lanadmin -a $LANID 2>/dev/null | awk '{print $4}' | sed s/0x//`
    echo ISN_MACAddress=$MACADDR

    # NIC speed
    SPEED=`lanadmin -s $LANID 2>/dev/null | awk '{print $3}'`
    SPEED=`expr $SPEED \/ 1000000`
    echo ISN_CurSpeed=$SPEED

    # MTU
    #MTU=`lanadmin -m $LANID 2>/dev/null | awk '{print $4}'`

    # Now get the description
    # -g not available on older versions of OS
    #DESCR=`lanadmin -g $LANID  2>/dev/null | grep ^Description |  awk -F= '{print $2}'`
    DESCR=`echo "lan display quit" | lanadmin $LANID 2>/dev/null | grep ^Description |  awk -F"= " '{print $2}'`
    echo DNIC_Model=`echo $DESCR | cut -f 2- -d " "`
    echo DNIC_Type=`echo "lan display quit" | lanadmin $LANID 2>/dev/null | grep ^Type |  awk '{print $4}'`
  fi

  # Need to get interface to provide to "netstat -ni -I xxx"
  IFACE=`lanscan -qi | awk -v a=$LANID '{if ($3 == a) print $1}'`
  if [ -z "$IFACE" ] ; then
    IFACE=`echo $DESCR | awk '{print $1}'`
  fi
  if [ -n "$IFACE" ] && [ "$netstat_check" -eq 1 ] ; then
    xx=`netstat -ni -I $IFACE | grep $IFACE`
    if [ -n "$xx" ] ; then
      echo ISN_DeviceID=$IFACE
      echo ISN_Identity=$IFACE
      IPADDR=`echo $xx | awk '{print $4}'`
      echo ISN_IPAddress=$IPADDR
    fi
  fi
  echo '</NETWORK>'
  echo ""
}

do_nic_hpux()
{
  if [ "$lanscan_check" -eq 1 ] ; then
      for i in `lanscan -q` ; do
    	  hpux_one_lan $i
      done
  fi
}

do_nic_linux()
{
	# Look for the first two lines for each NIC in the ifconfig output
	[ "$ifconfig_check" -eq 0 ] && return
	let state=0
	ifconfig -a |
	while read LINE ; do
		# Grab the second line for this card
		if [ "$state" -eq 1 ] ; then
			IPADDR=`expr "$LINE" : '.*addr:*\([^ ]*\)'`;
			ISN_SubnetMask=`expr "$LINE" : '.*Mask:*\([^ ]*\)'`;
			if [ -r /etc/sysconfig/network/routes ] ; then
				ISN_DefaultGateway=`grep "default" /etc/sysconfig/network/routes | awk '{print $2}'`;
			elif [ "$route_check" -eq 1 ] ; then
				ISN_DefaultGateway=`route | grep "default" | awk '{print $2}'`
			fi
			ISN_NameServer=`cat /etc/resolv.conf | grep "nameserver" | awk '{print $2}'`
			echo '<NETWORK>'
			if [ -n "$MACADDR" ]; then
				echo ISN_MACAddress=$MACADDR
			fi
			if [ -n "$ISN_SubnetMask" ]; then
				echo ISN_SubnetMask=$ISN_SubnetMask
			fi
			echo DNIC_Type=$TYPE

			CURSPEED=
			if [ "$TYPE" != "Local Loopback" ]; then
				if [ "$ethtool_check" -eq 1 ] ; then
					CURSPEED=`ethtool $IFACE 2>/dev/null | grep -v No\ data | grep Speed: | awk '{print $2}' | sed 's/Mb\/s//' | grep -v "[^0-9.]"`
				fi
				if [ "$mii_tool_check" -eq 1 ] && [ -z $CURSPEED ]; then
					CURSPEED=`mii-tool $IFACE 2>/dev/null | sed 's/baseT.*$//' | sed 's/Mbit.*$//' | awk '{print $NF}' | grep -v "[^0-9.]"`
				fi

				if [ "$lsdev_check" -eq 1 ] ; then
					IRQ=`lsdev | grep $IFACE | awk '{print $2}'`
				else
					IRQ=`grep $IFACE /proc/interrupts | awk '{print $1}' | sed s/://`
				fi
				if [ "$lspci_check" -eq 1 ] && [ -n "$IRQ" ] ; then
					NICMOD=`lspci -vvv | grep -B 5 "IRQ $IRQ" | head -1`
					NICMOD=`expr "$NICMOD" : '.*:.*: \(.*\)'`
				fi
				if [ -r $TMPDMESG ] ; then
					if [ -z "$NICMOD" ] ; then
						NICMOD=`egrep $IFACE:.*[0-9a-f]{2}:[0-9a-f]{2}:[0-9a-f]{2}: $TMPDMESG | sed 's/^.*'$IFACE':\ //' | sed 's/^.*\ at\ //' 2>/dev/null`
						if [ "$NICMOD" = "" ] ; then
							NICMOD=`grep $IFACE: $TMPDMESG | grep -v IPv6 | tail -1 | sed 's/^.*'$IFACE':\ //' 2>/dev/null`
						fi
						if [ "$NICMOD" = "" ] ; then
							NICMOD=$TYPE
						fi
					fi
					if [ -z $CURSPEED ]; then
						CURSPEED=`grep $IFACE\ NIC\ Link\ is\ Up $TMPDMESG | tail -1 | sed 's/^.*Up\ //' | sed 's/\ [A-Z][B,b]ps.*$//' | grep -v "[^0-9.]"`
					fi
				fi
				if [ -n "$NICMOD" ]; then
					echo DNIC_Model=$NICMOD
				fi
			else
				echo DNIC_Model=Local Loopback
			fi
			if [ -z $CURSPEED ]; then
				CURSPEED=0
			fi
			echo ISN_CurSpeed=$CURSPEED
			echo ISN_Identity=$IFACE
			echo ISN_DeviceID=$IFACE
			BOOTPROTO=
			if [ -r /etc/sysconfig/network-scripts/ifcfg-$IFACE ] ; then
				. /etc/sysconfig/network-scripts/ifcfg-$IFACE >/dev/null
			elif [ -r /etc/sysconfig/network/ifcfg-$IFACE ] ; then
				. /etc/sysconfig/network/ifcfg-$IFACE >/dev/null
			fi
			if [ -n "$GATEWAY" ] ; then
				ISN_DefaultGateway=$GATEWAY
			fi
			
			if [ -n "$ISN_DefaultGateway" ]; then
				echo ISN_DefaultGateway=$ISN_DefaultGateway
			fi

			if [ -n "$ISN_NameServer" ]; then
				echo ISN_NameServer=$ISN_NameServer
			fi

			if [ -n "$BOOTPROTO" ]; then
				echo ISN_IPAddrType=$BOOTPROTO
			else
				echo ISN_IPAddrType=Static
			fi
			if [ -n "$IPADDR" ]; then
				echo ISN_IPAddress=$IPADDR
			fi
			echo '</NETWORK>'
			echo ""
			let state=0
		fi

		# Looking for the first line for the card
		if [ "$state" -eq 0 ] ; then
			if expr "$LINE" : ".*Link encap" >/dev/null ; then
				TYPE=`expr "$LINE" : '.*encap:*\(.*\)' | sed 's/\ HWaddr.*$//'`
				IFACE=`echo "$LINE" | awk '{print $1}'`
				MACADDR=`expr "$LINE" : '.*HWaddr *\(.*\)'`
				let state=1
			fi
		fi
	done
}

do_nic_sun()
{
	[ "$ifconfig_check" -eq 0 ] && return
	ISN_NameServer=`cat /etc/resolv.conf | grep '^nameserver' | awk '{print $2}'`
	ISN_Domain=`cat /etc/resolv.conf | grep '^domain' | awk '{print $2}'`
	ISN_DefaultGateway=`cat /etc/defaultrouter`
	TMP_NICINFO=./nicinfo.$$
	ifconfig -a > $TMP_NICINFO 2>/dev/null
	WRITINGPROP=
	while read LINE; do
		FOUND=
		SPEED=
		if expr "$LINE" : ".*flags=" >/dev/null ; then
			if [ "$WRITINGPROP" = "1" ] ; then
				echo '</NETWORK>'
				echo ""
			fi
			echo '<NETWORK>'
			WRITINGPROP=1
			IFACE=`echo "$LINE" | awk '{print $1}' | tr -d :`
			echo ISN_Identity=$IFACE
			echo ISN_DeviceID=$IFACE
			FOUND=`expr "$LINE" : '\<.*DHCP.*\>'`
			if [ $FOUND -gt 0 ] ; then
				echo ISN_IPAddrType=DHCP
			else
				echo ISN_IPAddrType=Static
			fi
			FOUND=`expr "$LINE" : '\<.*LOOPBACK.*\>'`
			if [ $FOUND -gt 0 ] ; then
				echo DNIC_Type=Local Loopback
				echo DNIC_Model=Local Loopback
			else
				echo DNIC_Type=Broadcast
				if [ -n "$ISN_NameServer" ] ; then
					echo ISN_NameServer=$ISN_NameServer
				fi
				if [ -n "$ISN_Domain" ] ; then
					echo ISN_Domain=$ISN_Domain
				fi
				if [ -n "$ISN_DefaultGateway" ] ; then
					echo ISN_DefaultGateway=$ISN_DefaultGateway
				fi

				if [ "`echo "$IFACE" | awk '/^ce[0-9]+/ {print}'`" ] ; then
					INSTANCE=`echo "$IFACE" | cut -c 3-`
					if [ "$kstat_check" -eq 1 ] ; then
						SPEED=`kstat ce:$INSTANCE | grep link_speed | awk '{print $2}'`
					fi
				elif [ "`echo "$IFACE" | awk '/^bge[0-9]+/ {print}'`" ] ; then
					INSTANCE=`echo "$IFACE" | cut -c 4-`
					if [ "$kstat_check" -eq 1 ] ; then
						SPEED=`kstat bge:$INSTANCE:parameters | grep link_speed | awk '{print $2}'`
					fi
				elif [ "`echo "$IFACE" | awk '/^iprb[0-9]+/ {print}'`" ] ; then
					INSTANCE=`echo "$IFACE" | cut -c 5-`
					if [ "$kstat_check" -eq 1 ] ; then
						SPEED=`kstat iprb:$INSTANCE | grep ifspeed | awk '{print $2/1000000}'`
					fi
				elif [ "`echo "$IFACE" | awk '/^le[0-9]+/ {print}'`" ] ; then
					SPEED="10"
				else
					if [ "$ndd_check" -eq 1 ] ; then
						INTERFACE_TYPE=`echo "$IFACE" | sed -e "s/[0-9]*$//"`
						INSTANCE=`echo "$IFACE" | sed -e "s/^[a-z]*//"`
						ndd -set /dev/$INTERFACE_TYPE instance $INSTANCE > /dev/null
						SPEED=`ndd -get /dev/$INTERFACE_TYPE link_speed`
						if [ "$SPEED" = "0" ] ; then
							SPEED=10
						elif [ "$SPEED" = "1" ] ; then
							SPEED=100
						elif [ "$SPEED" != "1000" ] ; then
							SPEED=
						fi
					fi
				fi
				if [ -n "$SPEED" ] ; then
					echo DNIC_Speed=$SPEED
				fi
			fi

			if [ -r "$TMPDMESG" ] ; then
				FOUND=`grep ",$IFACE :" $TMPDMESG | grep Mbps | tail -1 | sed s/Mbps.*$// | awk '{print $NF}'`
				if [ -n "$FOUND" ] ; then
					 echo ISN_CurSpeed=$FOUND
				fi
			fi
		fi

		FOUND=`expr "$LINE" : '.*inet '`
		if [ $FOUND -gt 0 ] ; then
			 echo ISN_IPAddress=`expr "$LINE" : '.*inet \([^ ]*\)'`
		fi
		FOUND=`expr "$LINE" : '.*ether '`
		if [ $FOUND -gt 0 ] ; then
			 echo ISN_MACAddress=`expr "$LINE" : '.*ether \([^ ]*\)'`
		fi
	done < $TMP_NICINFO
	if [ -r "$TMP_NICINFO" ] ; then
		echo '</NETWORK>'
		echo ""
		rm -f $TMP_NICINFO
	fi
}

do_nic_aix()
{
	[ "$ifconfig_check" -eq 0 ] && return
	for IFACE in `ifconfig -l 2>/dev/null` ; do
		echo '<NETWORK>'
		LINE=`ifconfig $IFACE`
		echo ISN_Identity=$IFACE
		echo ISN_DeviceID=$IFACE
		if [ -r /etc/resolv.conf ] ; then
			NAMESVR=`grep '^nameserver' /etc/resolv.conf | awk '{print $2}'`
			if [ -n "$CGNAME" ] ; then
			    echo ISN_NameServer="$NAMESVR"
			fi
		fi
		echo ISN_Domain=`domainname`
		FOUND=`expr "$LINE" : '.*,DHCP,.*'`
		if [ $FOUND -gt 0 ] ; then
			echo ISN_IPAddrType=DHCP
		else
			echo ISN_IPAddrType=Static
		fi
		FOUND=`expr "$LINE" : '.*,LOOPBACK,.*'`
		if [ $FOUND -gt 0 ] ; then
			echo DNIC_Type=Local Loopback
			echo DNIC_Model=Local Loopback
		else
			echo DNIC_Type=Broadcast
			if [ "$entstat_check" -eq 1 ] ; then
				echo ISN_MACAddress=`entstat -d $IFACE | grep "Hardware Address" | awk '{print $3}'`
				echo DNIC_Model=`entstat -d $IFACE | grep "Device Type" | awk -F": " '{print $2}'`
				echo ISN_CurSpeed=`entstat -d $IFACE | grep "Speed Running" | awk '{print $4}'`
				DEV_TYPE=`entstat -d $IFACE | grep "Device Type"`
				echo DNIC_Make=`expr "$DEV_TYPE" : '.*: \([^ ]*\)'`
			fi
		fi
		FOUND=`expr "$LINE" : '.*inet '`
		if [ $FOUND -gt 0 ] ; then
			echo ISN_IPAddress=`expr "$LINE" : '.*inet \([^ ]*\)'`
		fi
		echo '</NETWORK>'
		echo ""
	done
}

do_nic()
{
	case "$OS_TYPE" in
	  HP-UX)
		 do_nic_hpux
		 ;;
	  Linux)
		 do_nic_linux
		 ;;
	  SunOS)
		 do_nic_sun
		 ;;
	  AIX)
		 do_nic_aix
		 ;;
	esac
}

# **************************************************************************
#  RAM Information Section
# **************************************************************************
get_ram_typenumber()
{
	case "$DRAM_Type" in
	  Unknown)			DRAM_Type=0		;;
	  Other)			DRAM_Type=1		;;
	  DRAM)			DRAM_Type=2		;;
	  "Synchronous DRAM")	DRAM_Type=3		;;
	  "Cache DRAM")		DRAM_Type=4		;;
	  EDO)			DRAM_Type=5		;;
	  EDRAM)			DRAM_Type=6		;;
	  VRAM)			DRAM_Type=7		;;
	  SRAM)			DRAM_Type=8		;;
	  RAM)			DRAM_Type=9		;;
	  ROM)			DRAM_Type=10	;;
	  Flash)			DRAM_Type=11	;;
	  EEPROM)			DRAM_Type=12	;;
	  FEPROM)			DRAM_Type=13	;;
	  EPROM)			DRAM_Type=14	;;
	  CDRAM)			DRAM_Type=15	;;
	  3DRAM)			DRAM_Type=16	;;
	  SDRAM)			DRAM_Type=17	;;
	  SGRAM)			DRAM_Type=18	;;
	  RDRAM)			DRAM_Type=19	;;
	  DDR)			DRAM_Type=20	;;
	  *)				DRAM_Type=0		;;
	esac
}

get_ram_formfactornumber()
{
	case "$DRAM_FormFactor" in
	  Unknown)		DRAM_FormFactor=0		;;
	  Other)		DRAM_FormFactor=1		;;
	  SIP)		DRAM_FormFactor=2		;;
	  DIP)		DRAM_FormFactor=3		;;
	  ZIP)		DRAM_FormFactor=4		;;
	  SOJ)		DRAM_FormFactor=5		;;
	  Proprietary)	DRAM_FormFactor=6		;;
	  SIMM)		DRAM_FormFactor=7		;;
	  DIMM)		DRAM_FormFactor=8		;;
	  TSOP)		DRAM_FormFactor=9		;;
	  PGA)		DRAM_FormFactor=10	;;
	  RIMM)		DRAM_FormFactor=11	;;
	  SODIMM)		DRAM_FormFactor=12	;;
	  SRIMM)		DRAM_FormFactor=13	;;
	  SMD)		DRAM_FormFactor=14	;;
	  SSMP)		DRAM_FormFactor=15	;;
	  QFP)		DRAM_FormFactor=16	;;
	  TQFP)		DRAM_FormFactor=17	;;
	  SOIC)		DRAM_FormFactor=18	;;
	  LCC)		DRAM_FormFactor=19	;;
	  PLCC)		DRAM_FormFactor=20	;;
	  BGA)		DRAM_FormFactor=21	;;
	  FPBGA)		DRAM_FormFactor=22	;;
	  LGA)		DRAM_FormFactor=23	;;
	  *)			DRAM_FormFactor=0		;;
	esac
}

do_ram_aix()
{
	[ "$lsdev_check" -eq 0 ] && return
	let slotnum=0;
	lsdev -Cc memory 2>/dev/null | grep "^mem" |
	while read INST STATUS HWPATH DESC; do
		echo '<RAM>'
		echo ISR_Identity=$INST
		echo ISR_SlotNumber=$slotnum
		if [ "$lsattr_check" -eq 1 ] ; then
			echo DRAM_Size=`lsattr -E -l $INST | grep "^size" | awk '{print $2}'`
		fi
		echo '</RAM>'
		echo ""
		let slotnum=slotnum+1
	done
}

do_ram_linux()
{
	let RAMCNT=0

	if [ "$dmidecode_check" -eq 1 ] ; then
		TMP_RAMINFO=$MYTMPDIR/ramtmp.$$
		if echo $sudocommands | grep -q ":dmidecode:" 
		then
			echo | sudo -S dmidecode | grep -A 12 'Memory Device$' > $TMP_RAMINFO 2>/dev/null
		else
			dmidecode | grep -A 12 'Memory Device$' > $TMP_RAMINFO 2>/dev/null
		fi
		
		if [ -r $TMP_RAMINFO ] ; then
			echo 'Memory Device' >> $TMP_RAMINFO
		fi

		ISR_Bank=
		ISR_Identity=
		DRAM_TotalWidth=
		DRAM_DataWidth=
		DRAM_Size=
		DRAM_FormFactor=
		DRAM_Type=
		DRAM_Speed=
		ISR_SerialNumber=

		while read F1 F2 F3 F4 F5; do
			case "$F1" in
			Bank)
				ISR_Bank="$F3 $F4 $F5"
				;;
			Locator:)
				ISR_Identity="$F2 $F3 $F4"
				;;
			Total)
				DRAM_TotalWidth=$F3
				;;
			Data)
				DRAM_DataWidth=$F3
				;;
			Size:)
				DRAM_Size=`expr "$F2" : "\([0-9]\{1,\}\)"`
				;;
			Form)
				DRAM_FormFactor=$F3
				get_ram_formfactornumber
				;;
			Type:)
				DRAM_Type=$F2
				get_ram_typenumber
				;;
			Speed:)
				DRAM_Speed=$F2
				;;
			Serial:)
				ISR_SerialNumber=$F3
				;;
			Memory)
				if [ -n "$DRAM_Size" ] ; then
					echo '<RAM>'
					if [ -z "$ISR_Identity" ] ; then
						ISR_Identity=$RAMCNT
					elif [ "$ISR_Identity" != "$ISR_Bank" ] ; then
						ISR_Identity="$RAMCNT:$ISR_Bank:$ISR_Identity"
					fi

					echo ISR_Identity=$ISR_Identity
					echo ISR_SlotNumber=$RAMCNT

					if [ -z "$ISR_SerialNumber" ] ; then
						ISR_SerialNumber=$ISR_SerialNumber
					fi

					if [ -n "$DRAM_TotalWidth" ] ; then
						echo DRAM_TotalWidth=$DRAM_TotalWidth
					fi

					if [ -n "$DRAM_DataWidth" ] ; then
						echo DRAM_DataWidth=$DRAM_DataWidth
					fi

					echo DRAM_Size=$DRAM_Size

					if [ -n "$DRAM_FormFactor" ] ; then
						echo DRAM_FormFactor=$DRAM_FormFactor
					fi

					if [ -n "$DRAM_Type" ] ; then
						echo DRAM_Type=$DRAM_Type
					fi
					if [ -n "$DRAM_Speed" ] ; then
						echo DRAM_Speed=$DRAM_Speed
					fi
					echo '</RAM>'
					echo ''

					let RAMCNT=$RAMCNT+1
					ISR_Identity=
					ISR_SerialNumber=
					DRAM_TotalWidth=
					DRAM_DataWidth=
					DRAM_Size=
					DRAM_FormFactor=
					DRAM_Type=
					DRAM_Speed=
				fi
				;;
			esac
		done < $TMP_RAMINFO
		if [ -r "$TMP_RAMINFO" ] ; then
			rm -f $TMP_RAMINFO
		fi
	fi

	if [ $RAMCNT -eq 0 ] ; then
		KBYTES=`grep MachineMem /proc/meminfo | awk '{print $2}'`
		if [ -z "$KBYTES" ] ; then
			KBYTES=`grep MemTotal /proc/meminfo | awk '{print $2}'`
		fi
		let DRAM_Size=KBYTES/1024

		echo '<RAM>'
		echo ISR_Identity=0
		echo ISR_SlotNumber=0
		echo DRAM_Size=$DRAM_Size
		echo '</RAM>'
		echo ""
	fi
}

do_ram_default()
{
	DRAM_Size=
	if [ "$OS_TYPE" = "HP-UX" ] ; then
		if [ "$getconf_check" -eq 1 ] ; then
			DRAM_Size=`getconf MEM_MBYTES 2>/dev/null`
			if [ -z "$DRAM_Size" ] && [ "$adb_check" -eq 1 ] ; then
				PAGESIZE=`getconf PAGESIZE`
				DRAM_Size=`echo phys_mem_pages/D | adb /stand/vmunix /dev/kmem | grep phys_mem_pages | tail -1 | awk -v a=$PAGESIZE '{print $2/1024/1024*a}' 2>/dev/null`
			fi
		fi
		if [ -z "$DRAM_Size" ] && [ "$cstm_check" -eq 1 ] ; then
			TMP_RAM=$MYTMPDIR/tmpramsize.$$
			echo "selclass qualifier cpu;info;wait;infolog"|cstm > $TMP_RAM 2>/dev/null
			DRAM_Size=`awk '/Total Configured Memory/ { print $5 }' ./$TMP_RAM`
			if [ -r "$TMP_RAM" ] ; then
				rm -f $TMP_RAM
			fi
		fi				
	elif [ "$OS_TYPE" = "SunOS" ] ; then
		[ "$prtconf_check" -eq 0 ] && return
		DRAM_Size=`prtconf | grep '^Memory' | awk '{print $3}'`
	fi

	echo '<RAM>'
	echo ISR_Identity=0
	echo ISR_SlotNumber=0
	echo DRAM_Size=$DRAM_Size
	echo '</RAM>'
	echo ""
}

do_ram()
{
	case "$OS_TYPE" in
	  HP-UX)
		 do_ram_default
		 ;;
	  Linux)
		 do_ram_linux
		 ;;

	  SunOS)
		 do_ram_default
		 ;;
	  AIX)
		 do_ram_aix
		 ;;
	esac
}

# **************************************************************************
#  Services/Daemon Information Section
# **************************************************************************
do_daemons_linux()
{
  [ "$runlevel_check" -eq 0 ] && return
  RUNLEVEL=`runlevel | awk '{print $2}'`
  ls -Al /etc/rc.d/rc$RUNLEVEL.d | grep ^lrwxrwxrwx |
  while read DAEMON ; do
    if [ -n "$DAEMON" ] ; then
      FNAME=`echo $DAEMON | awk '{print $9}'`
      RUNCMD=`echo /etc/rc.d/rc$RUNLEVEL.d/$FNAME`
      # DSTATUS=`$RUNCMD status 2>/dev/null`
	# ISACTIVE=`echo $DSTATUS | grep -i running | wc -l`
	ISACTIVE=1
	DISPNAME=`grep -A 8 "### BEGIN INIT INFO" $RUNCMD | grep "^# Provides:" | awk '{print $3}'`
	if [ "$DISPNAME" = "" ] ; then
		DISPNAME=$FNAME
	fi
	SERVDESC=`grep -A 8 "### BEGIN INIT INFO" $RUNCMD | grep "^# Description:" | cut -f3- -d " "`
	if [ "$SERVDESC" = "" ] ; then
		SERVDESC=$DISPNAME
	fi

      echo '<DAEMON>'
      echo DSVC_DisplayName=$DISPNAME
      echo DSVC_Name=$FNAME
      echo DSVC_Description=$SERVDESC
		echo DSVC_Type=2
      echo ISS_EXEPath=`echo $DAEMON | awk '{print $11}'`
      echo ISS_RunAccount=`echo $DAEMON | awk '{print $3}'`
      echo ISS_Identity=$FNAME
      echo ISS_IsActive=$ISACTIVE
      echo ISS_Status=$DSTATUS
      echo ISS_Identity=$FNAME
      echo '</DAEMON>'
      echo ""
    fi
  done
}

do_daemons_default()
{
	grep -v '^#' /etc/inetd.conf | awk '{print $1,$5,$6}' | sort | uniq |
	while read NAME OWNER EXEPATH ; do
		if [ -n "$NAME" ] ; then
			echo '<DAEMON>'
			echo DSVC_Name=$NAME
			echo DSVC_DisplayName=$NAME
			echo DSVC_Type=2
			echo ISS_Identity=$NAME::$EXEPATH
			echo ISS_EXEPath=$EXEPATH
			echo ISS_IsActive=1
			echo ISS_RunAccount=$OWNER
			echo '</DAEMON>'
			echo ""
		fi
	done
}

do_daemons()
{
	case "$OS_TYPE" in
	  HP-UX)
		 do_daemons_default
		 ;;
	  Linux)
		 do_daemons_linux
		 ;;
	  SunOS)
		 do_daemons_default
		 ;;
	  AIX)
		 do_daemons_default
		 ;;
	esac
}

# **************************************************************************
#  Shares/Exports Information Section
# **************************************************************************
do_exports()
{
	[ "$showmount_check" -eq 0 ] && return
	if echo $sudocommands | grep -q ":showmount:" 2>/dev/null
	then
		echo | sudo -S showmount -e 2>/dev/null | egrep -iv '^no|^export' |
		while read EXPORT PERMIS ; do
			if [ -n "$EXPORT" ] ; then
				echo '<EXPORTS>'
				echo ISSH_Identity=$EXPORT
				echo ISSH_NetName=$EXPORT
				echo ISSH_Type=NFS
				echo ISSH_Permissions=$PERMIS
				echo ISSH_Path=$EXPORT
				echo '</EXPORTS>'
				echo ""
			fi
		done
	else
		showmount -e 2>/dev/null | egrep -iv '^no|^export' |
		while read EXPORT PERMIS ; do
			if [ -n "$EXPORT" ] ; then
				echo '<EXPORTS>'
				echo ISSH_Identity=$EXPORT
				echo ISSH_NetName=$EXPORT
				echo ISSH_Type=NFS
				echo ISSH_Permissions=$PERMIS
				echo ISSH_Path=$EXPORT
				echo '</EXPORTS>'
				echo ""
			fi
		done
	fi
}

# **************************************************************************
#  Filesystems/Volumes Information Section
# **************************************************************************
do_volume()
{
	[ "$df_check" -eq 0 ] && return

	if [ "$OS_TYPE" = "HP-UX" ] ; then
		CMD="bdf -l"
		MNTTAB=/etc/mnttab
	elif [ "$OS_TYPE" = "Linux" ] ; then
		CMD="df -PkT -l"
		MNTTAB=/etc/mtab
	elif [ "$OS_TYPE" = "SunOS" ] ; then
		CMD="df -k -l"
		MNTTAB=/etc/mnttab
	elif [ "$OS_TYPE" = "AIX" ] ; then
		CMD="df -kP"
		MNTTAB=/etc/filesystems
	else
		return
	fi

	$CMD |
	( # Skip the header-line
	read FS TYPE KB USED AVAIL PCT MOUNTED 
	while read FS TYPE SIZE USED AVAIL PCT MOUNTED ; do
		# Determine the filesystem-type
		if [ -z "$SIZE" ] ; then
			read SIZE USED AVAIL PCT MOUNTED
		fi
		if [ "$OS_TYPE" = "AIX" ] ; then
			TYPE="jfs" 
			MNTOPT=
		else
			set -- `grep $FS $MNTTAB`
			#TYPE=$3
			MNTOPT=$4
		fi
		if [ "$SIZE" != "-" ]; then
			SIZE=`expr $SIZE \/ 1024`
			USED=`expr $USED \/ 1024`
			AVAIL=`expr $AVAIL \/ 1024`
			echo "<FILESYS>"
			echo ISFS_Device=$FS
			echo ISFS_Type=$TYPE
			echo ISFS_MountOpt=$MNTOPT
			echo ISFS_Size=$SIZE
			echo ISFS_SpaceUsed=$USED
			echo ISFS_SpaceFree=$AVAIL
			echo ISFS_PctUsed=`echo $PCT | tr -d %`
			echo ISFS_Path=$MOUNTED
			echo ISFS_Identity=$MOUNTED
			echo "</FILESYS>"
			echo ""
		fi
	done
	)
}

# **************************************************************************
#  Bus Device Information Section
# **************************************************************************
do_bus_hpux()
{
  [ "$ioscan_check" -eq 0 ] && return
  ioscan -fk 2>/dev/null | grep ^ba |
  while read CLASS INST HWPATH DRIVER SWSTATE HWTYPE DESCR ; do
    echo '<BUS>'
    echo ISB_Identity=$INST
    echo ISB_HWPath=$HWPATH
    echo DBUS_Driver=$DRIVER
    echo ISB_State=$SWSTATE
    echo DBUS_HWType=$HWTYPE
    echo DBUS_Description=$DESCR
    echo '</BUS>'
    echo ""
  done
}

do_bus_linux()
{
  [ "$lspci_check" -eq 0 ] && return  
  lspci |
  while read INST LINE ; do
    echo '<BUS>'
    echo ISB_Identity=$INST
    echo DBUS_HWType=`echo $LINE | awk -F: '{print $1}'`
    #echo HWPATH=$INST
    echo DBUS_Description=`echo $LINE | awk -F: '{print $2}' | sed 's/^ *//'`
    echo '</BUS>'
    echo ""
  done
}

do_bus_sun()
{
  [ "$prtdiag_check" -eq 0 ] && return
  prtdiag | grep PCI-1 |
  while read LINE ; do
    echo '<BUS>'
    echo ISB_Identity=$LINE
    echo DBUS_HWType=`echo $LINE | awk '{print $2}'`
    echo ISB_HWPath=`echo $LINE | awk '{print $1}'`
    echo DBUS_Description=`echo $LINE | awk '{print $5}'`
    echo '</BUS>'
    echo ""
  done
}

do_bus_aix()
{
  [ "$lsdev_check" -eq 0 ] && return  
  lsdev -Cc adapter |
  while read INST STATUS ID DESC ; do
    echo '<BUS>'
    echo ISB_Identity=$ID
    echo DBUS_HWType=$INST
    echo ISB_HWPath=$ID
    echo DBUS_Description=$DESC
    echo '</BUS>'
    echo ""
  done
}

do_bus()
{
	case "$OS_TYPE" in
	  HP-UX)
		 do_bus_hpux
		 ;;
	  Linux)
		 do_bus_linux
		 ;;
	  SunOS)
		 do_bus_sun
		 ;;
	  AIX)
		 do_bus_aix
		 ;;
	esac
}

# **************************************************************************
#  Main Routine
# **************************************************************************
PATH=/bin:/usr/bin:/usr/sbin:/sbin:/usr/contrib/bin
export PATH

LANG=C
export LANG
umask 022

# Holds all commands that should be run with sudo using : as separator
sudocommands=":"

# Directory to hold temporary files
MYTMPDIR=${MRMPDATADIR:-/tmp}
if [ ! -d "$MYTMPDIR" ] ; then
  exit
fi

# Initial setup
CONFIGFILE=$MRMPDIR/mrmpconfiginv.sh
if [ ! -r "$CONFIGFILE" ] ; then
	CONFIGFILE=./mrmpconfiginv.sh
fi
if [ ! -r "$CONFIGFILE" ] ; then
	CONFIGFILE=../mrmpconfiginv.sh
fi
if [ ! -r "$CONFIGFILE" ] ; then
	CONFIGFILE=/tmp/mrmpconfiginv.sh
fi
if [ -r "$CONFIGFILE" ]; then
	. $CONFIGFILE
fi

set -f

do_control

# Include Sun platform directory for eeprom and prtdiag
if [ "$OS_TYPE" = "SunOS" ]; then
	PLATDIR=/usr/platform/`uname -i`/sbin
	if [ -d "$PLATDIR" ]; then
		PATH=$PATH:$PLATDIR
		export PATH
	fi
fi

test_general_tools

do_test_tools

TMPDMESG=$MYTMPDIR/dmesginfo.$$
if [ -r /var/log/dmesg ]; then
	cat /var/log/dmesg > $TMPDMESG
elif [ -r /var/log/boot.msg ]; then
	cat /var/log/boot.msg > $TMPDMESG
elif [ -x /usr/bin/alog ]; then
	/usr/bin/alog -o -t boot > $TMPDMESG
else
      if [ "$dmesg_check" -eq 0 ] ; then
            dmesg > $TMPDMESG
	fi
fi

# Get System Information
if [ "$InvCollObjectExcl_1" != "1" ]; then
	do_sys
fi
# Get OS Information
if [ "$InvCollObjectExcl_2" != "1" ]; then
	do_os
fi
# Get App Information
if [ "$InvCollObjectExcl_3" != "1" ]; then
	do_apps
fi
# Get Chassis Information
if [ "$InvCollObjectExcl_4" != "1" ]; then
	do_chassis
fi
# Get CPU Information
if [ "$InvCollObjectExcl_5" != "1" ]; then
	do_cpu
fi
# Get Drive Adpt Information
if [ "$InvCollObjectExcl_6" != "1" ]; then
	do_driveadpt
fi
# Get Drive Information
if [ "$InvCollObjectExcl_7" != "1" ]; then
	do_drive
fi
# Get NIC Information
if [ "$InvCollObjectExcl_8" != "1" ]; then
	do_nic
fi
# Get RAM Information
if [ "$InvCollObjectExcl_9" != "1" ]; then
	do_ram
fi
# Get Service/Daemon Information
if [ "$InvCollObjectExcl_10" != "1" ]; then
	do_daemons
fi
# Get Share/Export Information
if [ "$InvCollObjectExcl_11" != "1" ]; then
	do_exports
fi
# Get Volume Information
if [ "$InvCollObjectExcl_12" != "1" ]; then
	do_volume
fi

rm -f $TMPDMESG
exit 0
