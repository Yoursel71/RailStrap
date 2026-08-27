import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";

const inter = Inter({ subsets: ["latin"], variable: "--font-inter" });

export const metadata: Metadata = {
  title: "RailStrap",
  description:
    "RailStrap is a free, open-source, third-party bootstrapper for Roblox with extra features, built for privacy and performance.",
  icons: { icon: "/logo.png" },
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <body className={`${inter.variable} bg-grid font-sans`}>{children}</body>
    </html>
  );
}
