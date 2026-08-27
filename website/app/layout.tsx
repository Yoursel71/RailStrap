import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";

const inter = Inter({ subsets: ["latin"], variable: "--font-inter" });

export const metadata: Metadata = {
  title: "RailStrap — A better Roblox launcher",
  description:
    "RailStrap is a free, open-source, third-party bootstrapper for Roblox with extra features, built for privacy and performance.",
  icons: { icon: "/logo.png" },
  openGraph: {
    title: "RailStrap — A better Roblox launcher",
    description: "A free, open-source Roblox bootstrapper for Windows, built for privacy and control.",
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
