Below is a **full, polished narrative script** you can read aloud during your demo.
It is written to sound natural, confident, and persuasive—like a real presenter speaking to stakeholders.

---

# **Full Demo Narrative Script (Read-Aloud Version)**

**[Opening – 30 seconds]**
*“Good morning everyone, and thanks for taking the time to join. Today I’m excited to show you something that will make a real difference in how we access and use yield curve data across the organization. We’ve built a unified Yield Curve API—a single, clean interface that brings consistency, speed, and reliability to something we use every day: yield curves.*

*This demo will walk through what the API does, why it matters, and how it can immediately simplify workflows across teams.”*

---

## **1. The Problem We’re Solving – 45 seconds**

*“Right now, our yield curve data comes from multiple places—files, spreadsheets, manual pulls, and often on a request-by-request basis. That means duplicated effort, inconsistent formats, and unnecessary operational risk. Different teams often end up maintaining their own versions of the same data.*

*What we need is a single authoritative source—simple, self-service, and standardized. That’s exactly what this API provides.”*

---

## **2. Feature Overview – 1 minute**

*“Let me quickly highlight what the API can do before we walk through it live.”*

* **List all yield curves by name**
  *“If you want to know what curves exist, the API provides a clean list. It even supports wildcard search—so if you only remember part of a curve name or a naming pattern, you can find it instantly.”*

* **List all dates with available data**
  *“For any curve, you can request all the dates for which data exists. This is extremely useful for analytics, historical work, or simply confirming data coverage.”*

* **Retrieve full yield curve data — in JSON or CSV**
  *“You can retrieve the full curve for any date in either JSON or CSV. That means it plugs directly into Python, Excel, R, dashboards, or any downstream system. No reformatting needed.”*

* **Support for multiple currencies**
  *“If we maintain curves in different currencies, the API handles that consistently. You request the currency you want, and the API returns the curve in a standardized structure. No more digging through files or reconciling formats.”*

---

## **3. Live Demo Script – 3 minutes**

### **Step 1 — Finding Curves**

*“Let’s start by discovering what curves are available. I’ll hit the ‘List Curves’ endpoint. As you can see, in one call, I get the full catalog of curves we maintain. And I can also enter something like a partial name—‘USD*’ for example—and the API returns all curves that match the pattern. This makes discovery extremely intuitive.”*

### **Step 2 — Checking Data Availability**

*“Next, let’s pick one of these curves and check what dates are available. I’ll call the ‘List Dates’ endpoint. Here we can see every date where we have data for this curve. This is great for back-testing, validation, or simply confirming historic coverage.”*

### **Step 3 — Retrieving a Curve**

*“Now let’s actually pull a curve. I’ll request the curve for a specific date. Here it is in JSON—standard field names, consistent structure, no ambiguity. And with one small parameter change, I can request the same data in CSV format, ready to drop directly into Excel or any data-processing pipeline.”*

### **Step 4 — Working Across Currencies**

*“And finally, let’s look at a curve in another currency. With a simple parameter, I specify the currency, and the API returns it in the same clean structure as everything else. Whether it's USD, EUR, GBP, or beyond, the format is uniform and the process is identical.”*

---

## **4. Why This Matters – 1 minute**

*“So why is this important? Because we’re moving from fragmented, manual, and inconsistent processes to a unified, automated, and reliable one. With this API, every team—trading, risk, quant, treasury, finance—can access the same curves in the same structure, instantly.*

*It removes friction. It reduces operational load. And it gives us a scalable foundation for analytics, modelling, reporting, and future automation.”*

---

## **5. Closing – 20 seconds**

*“In short, the Yield Curve API gives us a single source of truth—fast, clean, consistent, and accessible. I’m excited to roll this out more widely and support teams as they begin integrating it into their workflows. Thank you, and I’m happy to answer questions or walk through additional scenarios.”*

---

If you'd like, I can also produce:
✅ A shorter, more energetic pitch
✅ A more technical script for quant or engineering audiences
✅ A PowerPoint slide deck
Just tell me!
