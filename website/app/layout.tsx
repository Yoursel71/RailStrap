import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";
import { siteAsset } from "@/lib/site";

const inter = Inter({ subsets: ["latin"], variable: "--font-inter" });

export const metadata: Metadata = {
  metadataBase: new URL("https://yoursel71.github.io"),
  title: "RailStrap — A better Roblox launcher",
  description: "A private, open-source Roblox launcher for Windows.",
  icons: { icon: siteAsset("/logo.png") },
  openGraph: {
    title: "RailStrap — A better Roblox launcher",
    description: "A private, open-source Roblox launcher for Windows.",
    type: "website",
    images: [
      {
        url: siteAsset("/railstrap-artwork.png"),
        width: 1672,
        height: 941,
        alt: "Painted RailStrap artwork featuring two Roblox characters",
      },
    ],
  },
  twitter: {
    card: "summary_large_image",
    images: [siteAsset("/railstrap-artwork.png")],
  },
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <body className={`${inter.variable} bg-grid font-sans`}>{children}</body>
    </html>
  );
}
