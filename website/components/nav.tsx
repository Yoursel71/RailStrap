import Image from "next/image";
import Link from "next/link";
import { Github, Download } from "lucide-react";
import { Button } from "@/components/ui/button";

const links = [
  { href: "#features", label: "Features" },
  { href: "#download", label: "Download" },
  { href: "#privacy", label: "Privacy" },
  { href: "#help", label: "Help" },
];

export function Nav() {
  return (
    <header className="sticky top-0 z-50 border-b border-border bg-background/70 backdrop-blur-md">
      <nav className="container flex h-[60px] items-center justify-between">
        <Link href="#hero" className="flex items-center gap-2 text-[15px] font-bold tracking-tight">
          <Image src="/logo.png" alt="RailStrap logo" width={24} height={24} className="rounded-md" />
          RailStrap
        </Link>

        <div className="hidden gap-6 text-[13.5px] text-muted-foreground sm:flex">
          {links.map((l) => (
            <a key={l.href} href={l.href} className="transition-colors hover:text-foreground">
              {l.label}
            </a>
          ))}
        </div>

        <div className="flex items-center gap-2.5">
          <Button variant="ghost" size="sm" asChild>
            <a href="https://github.com/Yoursel71/RailStrap" target="_blank" rel="noopener noreferrer">
              <Github /> GitHub
            </a>
          </Button>
          <Button size="sm" asChild>
            <a href="https://github.com/Yoursel71/RailStrap/releases/latest" target="_blank" rel="noopener noreferrer">
              <Download /> Download
            </a>
          </Button>
        </div>
      </nav>
    </header>
  );
}
