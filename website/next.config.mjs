const isProd = process.env.NODE_ENV === "production";

/** @type {import('next').NextConfig} */
const nextConfig = {
  output: "export",
  images: { unoptimized: true },
  // Served as a project page at yoursel71.github.io/RailStrap
  basePath: isProd ? "/RailStrap" : "",
  assetPrefix: isProd ? "/RailStrap/" : "",
};

export default nextConfig;
