# Git History Cleanup Guide

## ⚠️ Important: Sensitive Data in Git History

Yes, even though the sensitive data has been removed from the current files, **it is still visible in the git history**. Anyone who clones or accesses the repository can view previous commits and see the original credentials.

## Why This Matters

Git stores the complete history of all changes. When you commit a file, Git saves a snapshot of that file at that point in time. Simply modifying the file in a new commit doesn't remove the old versions from history.

For example, running this command shows the old credentials:
```bash
git show 3a48c04:DragonBallLibrary.AppHost/appsettings.Development.json
```

## 🔍 What's Exposed in History

Based on the audit, the following sensitive data is visible in commit history:
- SQL Server password: `dr4g0n84!!`
- SQL Server username: `g0ku`
- Azure Subscription ID: `1c19e41d-1da6-4a71-ac80-bc9a7fbfb294`
- Azure Client ID: `53bf167e-f07b-4ee0-9c1f-96e9088bc6f0`
- Azure Tenant ID: `92cf6300-1c93-4b01-9a58-9603b66b404d`
- Azure Resource Names: `keyvault-5gvqxy7gckwwa`, `storage5gvqxy7gckwwa`, etc.

## ✅ Recommended Solutions

### Option 1: Clean Git History (Recommended for Public Release)

Use **BFG Repo-Cleaner** or **git-filter-repo** to rewrite git history and remove sensitive data.

#### Using BFG Repo-Cleaner (Easiest)

1. **Install BFG:**
   ```bash
   # On macOS
   brew install bfg
   
   # On Linux
   wget https://repo1.maven.org/maven2/com/madgag/bfg/1.14.0/bfg-1.14.0.jar
   alias bfg='java -jar bfg-1.14.0.jar'
   ```

2. **Create a fresh clone (mirror):**
   ```bash
   git clone --mirror https://github.com/santypr/aspire.git
   cd aspire.git
   ```

3. **Create a file with sensitive strings to remove:**
   ```bash
   cat > passwords.txt <<EOF
   dr4g0n84!!
   g0ku
   1c19e41d-1da6-4a71-ac80-bc9a7fbfb294
   53bf167e-f07b-4ee0-9c1f-96e9088bc6f0
   92cf6300-1c93-4b01-9a58-9603b66b404d
   keyvault-5gvqxy7gckwwa
   storage5gvqxy7gckwwa
   appconfiguration-5gvqxy7gckwwa
   rg-ga2025-tenerife
   EOF
   ```

4. **Run BFG to replace sensitive strings:**
   ```bash
   bfg --replace-text passwords.txt
   ```

5. **Clean up and force push:**
   ```bash
   git reflog expire --expire=now --all
   git gc --prune=now --aggressive
   git push --force
   ```

6. **Notify collaborators:** All collaborators must re-clone the repository.

#### Using git-filter-repo (More Powerful)

1. **Install git-filter-repo:**
   ```bash
   # On macOS
   brew install git-filter-repo
   
   # On Linux/Windows
   pip3 install git-filter-repo
   ```

2. **Create a fresh clone:**
   ```bash
   git clone https://github.com/santypr/aspire.git
   cd aspire
   ```

3. **Create a replacements file:**
   ```bash
   cat > replacements.txt <<EOF
   dr4g0n84!!==>***REMOVED***
   g0ku==>***REMOVED***
   1c19e41d-1da6-4a71-ac80-bc9a7fbfb294==>{your-azure-subscription-id}
   53bf167e-f07b-4ee0-9c1f-96e9088bc6f0==>{your-azure-client-id}
   92cf6300-1c93-4b01-9a58-9603b66b404d==>{your-azure-tenant-id}
   keyvault-5gvqxy7gckwwa==>{your-keyvault-name}
   storage5gvqxy7gckwwa==>{your-storage-account-name}
   appconfiguration-5gvqxy7gckwwa==>{your-app-config-name}
   rg-ga2025-tenerife==>{your-resource-group}
   EOF
   ```

4. **Run git-filter-repo:**
   ```bash
   git filter-repo --replace-text replacements.txt
   ```

5. **Re-add the remote and force push:**
   ```bash
   git remote add origin https://github.com/santypr/aspire.git
   git push --force --all origin
   git push --force --tags origin
   ```

6. **Notify collaborators:** All collaborators must delete their local copies and re-clone.

### Option 2: Create a Fresh Repository (Cleanest, but Loses History)

If you don't need the git history, the simplest approach is to create a completely new repository:

1. **Download the current state:**
   ```bash
   git clone https://github.com/santypr/aspire.git aspire-clean
   cd aspire-clean
   ```

2. **Remove git history:**
   ```bash
   rm -rf .git
   git init
   ```

3. **Create initial commit:**
   ```bash
   git add .
   git commit -m "Initial commit - cleaned repository for public release"
   ```

4. **Push to a new or existing repository:**
   ```bash
   git remote add origin https://github.com/santypr/aspire.git
   git push --force origin main
   ```

### Option 3: Archive Old Repo and Start Fresh (Safest)

1. **Rename the current repository** to `aspire-archive` (private)
2. **Create a brand new repository** called `aspire` (public)
3. **Copy only the clean files** from the current state to the new repository
4. **Initialize with a clean first commit**

This keeps the old repository as a private backup while providing a clean public version.

## 🔐 After Cleaning History

**CRITICAL:** Regardless of which method you choose, you MUST:

1. **✅ Rotate ALL exposed credentials immediately:**
   - Change SQL Server password (`dr4g0n84!!` is now public)
   - Delete and recreate the Azure Service Principal
   - Regenerate Azure Storage account keys
   - Update all services to use new credentials

2. **✅ Enable GitHub Security Features:**
   ```bash
   # Enable secret scanning
   # Go to: Settings > Security & analysis > Enable secret scanning
   
   # Enable push protection
   # Go to: Settings > Security & analysis > Enable push protection
   ```

3. **✅ Notify all collaborators:**
   - They must delete their local clones
   - Re-clone the repository after force push
   - Update any local credentials

## ⚠️ Important Warnings

1. **Force push is destructive:** Anyone who has cloned the repository will need to re-clone after you force push.

2. **Forks are affected:** If anyone has forked your repository, their forks will still contain the sensitive data. Contact GitHub support if needed.

3. **Cached copies:** Search engines and GitHub caches may have indexed old commits. Consider:
   - Making the repo temporarily private during cleanup
   - Waiting 24-48 hours after cleanup before making public
   - Contacting GitHub support to clear their cache if needed

4. **No going back:** Once you force push, the old history is gone (unless someone still has a copy).

## 📋 Recommended Workflow

For your situation, I recommend:

1. **First:** Rotate all exposed credentials immediately (do this now!)
2. **Then:** Use BFG Repo-Cleaner to clean the git history (easiest option)
3. **Next:** Verify no sensitive data remains: `git log --all -p | grep -i "dr4g0n84\|g0ku\|53bf167e"`
4. **Finally:** Enable GitHub secret scanning and push protection before making public

## 🆘 Need Help?

- **BFG Documentation:** https://rtyley.github.io/bfg-repo-cleaner/
- **git-filter-repo Documentation:** https://github.com/newren/git-filter-repo
- **GitHub Support:** If you need help clearing GitHub's cache or dealing with forks

## Additional Resources

- [GitHub: Removing sensitive data from a repository](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/removing-sensitive-data-from-a-repository)
- [Git Tools - Rewriting History](https://git-scm.com/book/en/v2/Git-Tools-Rewriting-History)
- [How to remove sensitive data from GitHub](https://stackoverflow.com/questions/872565/how-to-remove-file-from-git-history)
