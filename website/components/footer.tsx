import Image from "next/image";

const links = [
  { href: "https://github.com/Yoursel71/RailStrap", label: "GitHub" },
  { href: "https://github.com/Yoursel71/RailStrap/releases/latest", label: "Releases" },
  { href: "https://github.com/Yoursel71/RailStrap/issues", label: "Issues" },
  { href: "https://github.com/Yoursel71/RailStrap/blob/main/LICENSE", label: "License" },
];

export function Footer() {
  return (
    <footer className="border-t border-border py-11">
      <div className="container flex flex-wrap items-start justify-between gap-6">
        <div>
          <div className="mb-2.5 flex items-center gap-2 text-[15px] font-bold">
            <Image src="/logo.png" alt="" width={24} height={24} className="rounded-md" />
            RailStrap
          </div>
          <p className="max-w-[46ch] text-[13px] text-muted-foreground">
            An independent, personal fork of{" "}
            <a
              href="https://github.com/bloxstraplabs/bloxstrap"
              target="_blank"
              rel="noopener noreferrer"
              className="underline underline-offset-2"
            >
              Bloxstrap
            </a>{" "}
            by pizzaboxer, used under the MIT License. Not affiliated with Roblox Corporation or
            Bloxstrap Labs.
          </p>
        </div>
        <div className="flex flex-wrap gap-5 text-[13px] text-muted-foreground">
          {links.map((l) => (
            <a
              key={l.href}
              href={l.href}
              target="_blank"
              rel="noopener noreferrer"
              className="transition-colors hover:text-foreground"
            >
              {l.label}
            </a>
          ))}
        </div>
      </div>
    </footer>
  );
}
