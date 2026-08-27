export const siteBasePath = process.env.NODE_ENV === "production" ? "/RailStrap" : "";

export function siteAsset(path: string) {
  return `${siteBasePath}${path}`;
}
