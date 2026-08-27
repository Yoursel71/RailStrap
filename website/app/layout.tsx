import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";
import { siteAsset } from "@/lib/site";

const inter = Inter({ subsets: ["latin"], variable: "--font-inter" });

export const metadata: Metadata = {
  title: "RailStrap — A better Roblox launcher",
  description: "A private, open-source Roblox launcher for Windows.",
  icons: { icon: siteAsset("/logo.png") },
  openGraph: {
    title: "RailStrap — A better Roblox launcher",
    description: "A private, open-source Roblox launcher for Windows.",
    type: "website",
  },
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <body className={`${inter.variable} bg-grid font-sans`}>{children}</body>
    </html>
  );
}
