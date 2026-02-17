# Azure Storage

## Storage Account Types and Replication Options

Azure Storage account types — overview and guidance

- General-purpose v2 (StorageV2, recommended)
    - Supports blobs, files, queues, tables, and Data Lake Storage Gen2 (hierarchical namespace).
    - Supports access tiers (Hot/Cool/Archive), lifecycle management, all redundancy options, and newest features/SDKs.
    - Use for most new workloads for flexibility and feature parity.

- General-purpose v1 (classic / GPv1)
    - Older SKU with fewer features and no access tiering or many newer optimizations.
    - Consider migrating to GPv2 unless constrained by legacy requirements.

- Blob Storage (specialized)
    - Optimized for blob-only scenarios and historically used for tiering (Hot/Cool).
    - Functionally superseded by StorageV2; use only for legacy compatibility or specific billing cases.

- Block Blob Storage (Premium)
    - Premium storage optimized for low-latency, high-throughput block blob workloads (hot data).
    - Choose when you need high IOPS/throughput for blob I/O.

- FileStorage (Premium Files)
    - Premium SMB/NFS file shares for high-performance lifting-and-shifting of file servers and I/O‑intensive workloads.
    - Use when SMB/NFS semantics and low latency/guaranteed throughput are required.

- Decision guidance
    - Default to StorageV2 for general use and when you need access tiers, lifecycle rules, or ADLS Gen2.
    - Pick premium/specialized account types (BlockBlob/FileStorage) for latency/throughput-sensitive workloads.
    - Use specialized blob-only or GPv1 accounts only for cost/compatibility reasons; otherwise migrate to StorageV2.
    - Consider encryption, network restrictions, identity integration, and redundancy options when provisioning.

**Replication Options**

- **LRS (Locally Redundant Storage)**
    - Stores three synchronous copies within a single region (within a single location).
    - Lowest cost; protects against hardware failures but not datacenter- or region-level outages.
    - Use for non-critical, cost-sensitive workloads or when data residency requires a single region.

- **ZRS (Zone-Redundant Storage)**
    - Synchronously replicates data across multiple availability zones within the same region.
    - Higher availability and resilience to datacenter/zone failures than LRS.
    - Good for region-local high-availability workloads that require low latency and faster recovery.

- **GRS (Geo-Redundant Storage)**
    - Replicates data asynchronously to a paired secondary region (three copies in primary + three copies in secondary).
    - Protects against complete regional outages; secondary cannot be read by default.
    - Use for disaster recovery scenarios where cross-region resilience is required and eventual consistency is acceptable.

- **RA-GRS (Read-Access Geo-Redundant Storage)**
    - Same replication as GRS but enables read access to the secondary region (read-only).
    - Useful for geo-read scenarios or to offload read traffic during primary outages or maintenance.

- **GZRS (Geo-zone-redundant Storage)**
    - Combines zone-redundant replication in the primary region with asynchronous geo-replication to a secondary region.
    - Provides both zone-level durability and cross-region disaster protection.

- **RA-GZRS (Read-Access Geo-Zone-Redundant Storage)**
    - GZRS with read access enabled on the secondary region.
    - Suitable when you need both primary-region zone resilience and read access to the geo-secondary.

Considerations and guidance:
- Cost increases from LRS → ZRS → GRS/RA-GRS → GZRS/RA-GZRS; choose based on availability, durability, and budget.
- Geo-replicated options are eventually consistent for the secondary; application design must tolerate replication lag.
- Read-access options (RA-GRS/RA-GZRS) let you serve reads from the secondary but write operations remain against the primary.
- Not all replication options are available in all regions/account SKUs—verify support for your account type and region.
- For automated business-continuity recovery, pair replication choice with documented failover/runbook and testing (account failover is manual and must be planned).
- Recommend:
    - LRS for cost-sensitive, non-critical data.
    - ZRS for high availability within a region.
    - RA-GRS/RA-GZRS or GRS/GZRS for cross-region disaster recovery needs; pick RA-* if you need read access to the secondary.
- Also consider compliance, data residency, RTO/RPO targets, and backup/replication testing when selecting replication.

**Best Practices:**
- Choose replication based on durability and compliance needs.
- Use GRS for disaster recovery, LRS for cost-sensitive workloads.

---

## Blob Storage: Containers, Blob Types, Access Tiers

- **Containers**: Logical grouping for blobs.
- **Blob Types**: Block blobs (files), Append blobs (logs), Page blobs (disks).
- **Access Tiers**: Hot (frequent access), Cool (infrequent), Archive (rare).

**Example: Uploading a Blob**

**Java:**
```java
BlobClient blobClient = new BlobServiceClientBuilder()
		.connectionString("<connection-string>")
		.buildClient()
		.getBlobContainerClient("mycontainer")
		.getBlobClient("myfile.txt");
blobClient.uploadFromFile("localfile.txt");
```

**C#:**
```csharp
var blobServiceClient = new BlobServiceClient("<connection-string>");
var containerClient = blobServiceClient.GetBlobContainerClient("mycontainer");
var blobClient = containerClient.GetBlobClient("myfile.txt");
await blobClient.UploadAsync("localfile.txt");
```

**Node.js:**
```javascript
const { BlobServiceClient } = require("@azure/storage-blob");
const blobServiceClient = BlobServiceClient.fromConnectionString("<connection-string>");
const containerClient = blobServiceClient.getContainerClient("mycontainer");
const blockBlobClient = containerClient.getBlockBlobClient("myfile.txt");
await blockBlobClient.uploadFile("localfile.txt");
```

**Best Practices:**
- Use containers to organize blobs.
- Select access tier based on usage pattern.
- Use lifecycle policies to automate tier transitions.

---

## File Storage for Cloud File Shares

Azure Files provides fully managed file shares accessible via SMB and REST.

**Example: Mounting Azure File Share (Windows):**
```powershell
net use Z: \\<storage-account>.file.core.windows.net\<share-name> /u:<storage-account> <account-key>
```

**Best Practices:**
- Use Azure Files for lift-and-shift scenarios.
- Secure file shares with Azure AD and network restrictions.

---

## Storage Access Control: Keys, SAS Tokens, Azure AD Integration

- **Keys**: Full access to storage account.
- **SAS Tokens**: Granular, time-limited access.
- **Azure AD**: Role-based access for managed identities.

**Example: Generating SAS Token**

**Java:**
```java
BlobServiceSasSignatureValues values = new BlobServiceSasSignatureValues(
		OffsetDateTime.now().plusHours(1),
		BlobContainerSasPermission.parse("r")
);
String sasToken = blobClient.generateSas(values);
```

**C#:**
```csharp
var sasBuilder = new BlobSasBuilder
{
		BlobContainerName = "mycontainer",
		Resource = "c",
		ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
};
sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);
string sasToken = blobServiceClient.GenerateSasUri(sasBuilder);
```

**Node.js:**
```javascript
const { generateBlobSASQueryParameters, BlobSASPermissions } = require("@azure/storage-blob");
const sasToken = generateBlobSASQueryParameters({
	containerName: "mycontainer",
	permissions: BlobSASPermissions.parse("r"),
	expiresOn: new Date(new Date().valueOf() + 3600 * 1000)
}, sharedKeyCredential).toString();
```

**Best Practices:**
- Use SAS tokens for delegated access.
- Rotate keys regularly.
- Prefer Azure AD for enterprise scenarios.

---

## Blob Lifecycle Management and Archival Policies

Automate blob tier transitions and deletions using lifecycle management.

**Example: Lifecycle Policy (JSON):**
```json
{
	"rules": [
		{
			"enabled": true,
			"name": "move-to-cool",
			"type": "Lifecycle",
			"definition": {
				"filters": { "blobTypes": ["blockBlob"], "prefixMatch": ["logs/"] },
				"actions": { "baseBlob": { "tierToCool": { "daysAfterModificationGreaterThan": 30 } } }
			}
		}
	]
}
```

**Best Practices:**
- Use lifecycle policies to optimize storage costs.
- Archive rarely accessed data.
- Delete expired blobs automatically.

---

## Cost Optimization Strategies

- Choose appropriate access tiers.
- Use lifecycle management to move/clean up blobs.
- Monitor usage and set alerts.

**Best Practices:**
- Regularly review storage metrics.
- Use reserved capacity for predictable workloads.
- Clean up unused resources.
