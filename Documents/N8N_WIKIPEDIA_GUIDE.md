# n8n Pipeline Guide: Wikipedia to S3 & MongoDB

This guide walks you through perfectly executing the PBI requirements inside your n8n UI.

## 1. Node Setup Flow
Drag and drop these nodes onto your n8n canvas in this exact order:
1. **Webhook / Manual Trigger**: Starts the pipeline. Pass in JSON with `topic`, `theme`, `category`, and `playlist`.
2. **HTTP Request (Wikipedia)**: Fetches the article.
3. **AWS S3 (MinIO) - RAW**: Saves the original text.
4. **Code Node**: Cleans the text and generates metadata.
5. **AWS S3 (MinIO) - FORMATTED**: Saves the clean text.
6. **MongoDB**: Upserts the metadata.

---

## 2. HTTP Request Node (Wikipedia API)
We need to fetch the clean extract and revision ID.
* **Method**: `GET`
* **URL**: `https://en.wikipedia.org/w/api.php`
* **Query Parameters**:
  * `action`: `query`
  * `prop`: `extracts|revisions`
  * `exintro`: `false`
  * `explaintext`: `true`
  * `titles`: `={{ $json.topic }}`
  * `rvprop`: `ids`
  * `format`: `json`

---

## 3. The Code Node (The Cleaner)
Add a **Code Node** right after the HTTP Request. Paste this exact JavaScript. It strips footnotes, cleans phonetics, generates deterministic S3 paths, and builds the CC BY-SA 4.0 metadata for MongoDB.

```javascript
let items = $input.all();

for (let i = 0; i < items.length; i++) {
    // 1. Navigate Wikipedia's annoying nested JSON response
    let pages = items[i].json.query.pages;
    let pageId = Object.keys(pages)[0];
    let page = pages[pageId];

    if (pageId === "-1") {
        throw new Error("Article not found or disambiguation!");
    }

    let rawText = page.extract || "";
    let revisionId = page.revisions[0].revid.toString();
    let title = page.title;

    // 2. CLEANUP: Strip citations like [1], [23]
    let cleanText = rawText.replace(/\[\d+\]/g, "");
    
    // 3. CLEANUP: Strip phonetic pronunciations often found in first paragraphs (e.g. (/əˈɡʌstəs/))
    cleanText = cleanText.replace(/\s*\([^)]*?\/[^)]*?\)/g, "");
    
    // 4. CLEANUP: Remove Wiki Section Headers like == History ==
    cleanText = cleanText.replace(/==.*?==/g, ""); 
    
    // 5. Trim excess whitespace
    cleanText = cleanText.replace(/\n\s*\n/g, '\n').trim();

    // 6. Generate Deterministic S3 Keys
    let slug = title.toLowerCase().replace(/[^a-z0-9]+/g, "-");
    let lang = "en"; // Can be dynamic later
    let rawKey = `raw/${lang}/${slug}/${revisionId}.txt`;
    let formattedKey = `formatted/${lang}/${slug}/${revisionId}.txt`;

    // 7. Map the Final Output for MinIO and MongoDB
    items[i].json = {
        topic: items[i].json.topic,
        theme: items[i].json.theme || "General",
        category: items[i].json.category || "Uncategorized",
        playlist: items[i].json.playlist || "Default",
        
        article_title: title,
        language: lang,
        revision_id: revisionId,
        source_url: `https://${lang}.wikipedia.org/wiki/${encodeURIComponent(title)}`,
        
        license: "CC BY-SA 4.0",
        license_url: "https://creativecommons.org/licenses/by-sa/4.0/",
        modified: true,
        retrieved_at: new Date().toISOString(),
        
        s3_raw_key: rawKey,
        s3_formatted_key: formattedKey,
        
        raw_text: rawText,
        clean_text: cleanText
    };
}
return items;
```

---

## 4. AWS S3 Nodes (MinIO)
You will need two S3 nodes.
* **Credentials**: Create an AWS S3 credential. Enter your MinIO URL (`http://minio:9000`), Access Key, and Secret. Force Path Style = `true`.
* **S3 Node 1 (Raw)**:
  * Operation: `Upload`
  * Bucket Name: `somnipedia`
  * File Name: `={{ $json.s3_raw_key }}`
  * File Content: `={{ $json.raw_text }}`
* **S3 Node 2 (Formatted)**:
  * Operation: `Upload`
  * Bucket Name: `somnipedia`
  * File Name: `={{ $json.s3_formatted_key }}`
  * File Content: `={{ $json.clean_text }}`

---

## 5. MongoDB Node
This node strictly fulfills the CC BY-SA 4.0 license retention.
* **Operation**: `Update` (Set to **Upsert** so it updates if it exists, creates if not).
* **Collection**: `articles`
* **Update Key (Query)**: `{"source_url": "={{ $json.source_url }}"}`
* **Fields to Set**: Map all the properties (theme, category, playlist, article_title, revision_id, license, s3 keys, etc.) from the Code Node output.

**Done!** Your pipeline will now securely extract, scrub, upload to S3, and legally attribute everything in MongoDB perfectly in sync.
