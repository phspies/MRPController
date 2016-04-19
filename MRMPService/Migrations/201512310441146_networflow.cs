namespace MRPService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class networflow : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Credentials",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 50),
                        description = c.String(maxLength: 100),
                        username = c.String(maxLength: 30),
                        password = c.String(maxLength: 30),
                        domain = c.String(maxLength: 30),
                        credential_type = c.Int(nullable: false),
                        human_type = c.String(maxLength: 30),
                        hash_value = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 50),
                        status = c.String(maxLength: 30),
                        severity = c.Int(),
                        component = c.String(maxLength: 50),
                        summary = c.String(maxLength: 50),
                        timestamp = c.DateTime(),
                        entity = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.NetworkFlows",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 50),
                        source_address = c.String(maxLength: 50),
                        target_address = c.String(maxLength: 50),
                        source_port = c.Int(nullable: false),
                        target_port = c.Int(nullable: false),
                        protocol = c.Int(nullable: false),
                        timestamp = c.DateTime(nullable: false),
                        start_timestamp = c.DateTime(nullable: false),
                        stop_timestamp = c.DateTime(nullable: false),
                        packets = c.Int(nullable: false),
                        kbyte = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Performances",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 50),
                        workload_id = c.String(maxLength: 50),
                        timestamp = c.DateTime(nullable: false),
                        category_name = c.String(maxLength: 100),
                        counter_name = c.String(maxLength: 100),
                        instance = c.String(maxLength: 100),
                        value = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Platforms",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 50),
                        description = c.String(maxLength: 100),
                        username = c.String(maxLength: 50),
                        password = c.String(maxLength: 50),
                        datacenter = c.String(maxLength: 50),
                        vendor = c.Int(nullable: false),
                        url = c.String(maxLength: 50),
                        credential_id = c.String(maxLength: 50),
                        passwordok = c.Int(),
                        lastupdated = c.DateTime(),
                        platform_details = c.String(maxLength: 50),
                        human_vendor = c.String(maxLength: 50),
                        workload_count = c.Int(),
                        vlan_count = c.Int(),
                        networkdomain_count = c.Int(),
                        platform_version = c.String(maxLength: 50),
                        moid = c.String(maxLength: 50),
                        workload_sha1 = c.String(maxLength: 50),
                        vlan_sha1 = c.String(maxLength: 50),
                        networkdomain_sha1 = c.String(maxLength: 50),
                        hash_value = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Workloads",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 50),
                        hostname = c.String(maxLength: 50),
                        platform_id = c.String(maxLength: 50),
                        credential_id = c.String(maxLength: 50),
                        hash_value = c.String(maxLength: 50),
                        failovergroup_id = c.String(maxLength: 50),
                        moid = c.String(maxLength: 50),
                        enabled = c.Boolean(),
                        vcpu = c.Int(),
                        vcore = c.Int(),
                        vmemory = c.Int(),
                        storage_count = c.Long(),
                        credential_ok = c.Boolean(nullable: false),
                        application = c.String(maxLength: 50),
                        osedition = c.String(maxLength: 50),
                        ostype = c.String(maxLength: 50),
                        last_contact_attempt = c.DateTime(),
                        last_contact_status = c.Int(),
                        last_contact_message = c.String(maxLength: 50),
                        failed_contact_attempts = c.Int(),
                        iplist = c.String(maxLength: 255),
                        cpu_coresPerSocket = c.Int(),
                        perf_collection = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Workloads");
            DropTable("dbo.Platforms");
            DropTable("dbo.Performances");
            DropTable("dbo.NetworkFlows");
            DropTable("dbo.Events");
            DropTable("dbo.Credentials");
        }
    }
}
