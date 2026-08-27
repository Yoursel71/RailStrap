import fs from "node:fs";
import path from "node:path";

/**
 * Reads the live app version straight out of RailStrap.csproj at build time,
 * so the website never drifts from what ships in the actual release.
 */
export function getAppVersion(): string {
  try {
    const csprojPath = path.join(process.cwd(), "..", "RailStrap", "RailStrap.csproj");
    const contents = fs.readFileSync(csprojPath, "utf-8");
    const match = contents.match(/<Version>([^<]+)<\/Version>/);
    return match?.[1] ?? "0.0.0";
  } catch {
    return "0.0.0";
  }
}
